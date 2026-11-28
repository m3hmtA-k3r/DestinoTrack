using DestinoTrack.Business.Consts;
using System.Collections.Frozen;
using DestinoTrack.Business.Validators.Common;
using DestinoTrack.DataAccess.Repositories.Countries;
using DestinoTrack.DataAccess.Repositories.Customers;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.CustomerDtos;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Customers
{
    public class CustomerService(ICustomerRepository _customerRepository,
                             ICountryRepository _countryRepository,
                             ICustomerCodeGenerator _customerCodeGenerator,
                             IStringLocalizer<SharedResource> _localizer) : ICustomerService
    {
        // İzin verilen geçişler kodda durur, veritabanından değiştirilemez (CargoService kalıbı).
        // Listede olmayan her geçiş yasaktır — Closed son durumdur
        private static readonly FrozenDictionary<AccountStatus, AccountStatus[]> AllowedTransitions =
            new Dictionary<AccountStatus, AccountStatus[]>
            {
                [AccountStatus.PendingApproval] = [AccountStatus.Active, AccountStatus.Closed],
                [AccountStatus.Active] = [AccountStatus.Suspended, AccountStatus.Closed],
                [AccountStatus.Suspended] = [AccountStatus.Active, AccountStatus.Closed],
                [AccountStatus.Closed] = []
            }.ToFrozenDictionary();

        public async Task<PagedResult<ResultCustomerDto>> GetPagedAsync(AccountStatus? status = null, CustomerType? customerType = null,
                                                                        string? search = null, int page = 1)
        {
            page = page < 1 ? 1 : page;

            var (customers, totalCount) = await _customerRepository.GetPagedAsync(status, customerType, search, page, Paging.PageSize);

            // Son sayfadaki tek kayıt silinince o sayfa boş kalır → varsa yeni son sayfa gösterilir
            if (customers.Count == 0 && totalCount > 0)
            {
                page = (int)Math.Ceiling(totalCount / (double)Paging.PageSize);
                (customers, totalCount) = await _customerRepository.GetPagedAsync(status, customerType, search, page, Paging.PageSize);
            }

            // Gönderi sayısı ve son gönderi tarihi YALNIZ bu sayfanın müşterileri için, tek sorguda
            var istatistik = await _customerRepository.GetCargoStatsAsync(customers.Select(c => c.Id).ToList());

            return new PagedResult<ResultCustomerDto>
            {
                Items = customers.Select(c =>
                {
                    // Hiç gönderisi olmayan müşteri sözlükte yok: demet varsayılanı (0, null) kalır
                    istatistik.TryGetValue(c.Id, out var istat);

                    return new ResultCustomerDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Code = c.Code,
                        TaxNumber = c.TaxNumber,
                        CustomerType = c.CustomerType,
                        Status = c.Status,
                        CountryName = c.Country?.Name,
                        CargoCount = istat.CargoCount,
                        LastCargoDate = istat.LastCargoDate
                    };
                }).ToList(),
                Page = page,
                PageSize = Paging.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<List<CustomerStatusFilterDto>> GetStatusFiltersAsync()
        {
            var sayimlar = await _customerRepository.GetCountsByStatusAsync();

            // Dört durumun HEPSİ dönüyor: sayısı sıfır olan çip de gösterilir
            return Enum.GetValues<AccountStatus>()
                .Select(durum => new CustomerStatusFilterDto
                {
                    Status = durum,
                    CustomerCount = sayimlar.TryGetValue(durum, out var adet) ? adet : 0
                })
                .ToList();
        }

        public async Task<CustomerPortfolioDto> GetPortfolioAsync()
        {
            var sayimlar = await _customerRepository.GetCountsByStatusAsync();
            var (activeCorporate, newThisMonth) = await _customerRepository.GetPortfolioCountsAsync();

            return new CustomerPortfolioDto
            {
                // İkisi de durum sayımlarından TÜRETİLİYOR, ayrıca sorgulanmıyor
                TotalCustomers = sayimlar.Values.Sum(),
                PendingApproval = sayimlar.TryGetValue(AccountStatus.PendingApproval, out var bekleyen) ? bekleyen : 0,

                ActiveCorporate = activeCorporate,
                NewThisMonth = newThisMonth
            };
        }


        public async Task<UpdateCustomerDto> GetByIdAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id)
                           ?? throw new ValidationException(_localizer["CustomerNotFound"].Value);

            return new UpdateCustomerDto
            {
                Id = customer.Id,
                Title = customer.Title,
                CustomerType = customer.CustomerType,
                CountryId = customer.CountryId,
                TaxNumber = customer.TaxNumber,
                TaxOffice = customer.TaxOffice,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,

                // Salt okunur: yalnız ekranda gösterilir, UpdateAsync bunları yazmaz
                Code = customer.Code,
                Status = customer.Status
            };
        }

        public async Task CreateAsync(CreateCustomerDto createCustomerDto)
        {
            var taxNumber = await ValidateTaxNumberAsync(createCustomerDto.CountryId, createCustomerDto.CustomerType,
                                                         createCustomerDto.TaxNumber, excludeCustomerId: null);

            var customer = new Customer
            {
                Code = await _customerCodeGenerator.GenerateAsync(),
                Title = createCustomerDto.Title.Trim(),
                CustomerType = createCustomerDto.CustomerType,
                TaxNumber = taxNumber,

                // Bireyselde vergi dairesi sorulmuyor — kayıt formundaki kuralın aynısı
                TaxOffice = string.IsNullOrWhiteSpace(createCustomerDto.TaxOffice) ? "-" : createCustomerDto.TaxOffice.Trim(),

                Email = createCustomerDto.Email.Trim(),
                PhoneNumber = createCustomerDto.PhoneNumber.Trim(),

                // D115 · admin zaten onay makamı: kendi açtığı müşteri beklemeye düşmez
                Status = AccountStatus.Active,
                CountryId = createCustomerDto.CountryId
            };

            await _customerRepository.CreateAsync(customer);
        }

        public async Task UpdateAsync(UpdateCustomerDto updateCustomerDto)
        {
            var customer = await _customerRepository.GetByIdAsync(updateCustomerDto.Id)
                           ?? throw new ValidationException(_localizer["CustomerNotFound"].Value);

            // Kaydın kendisi çakışma sayılmaz: numarayı değiştirmeden kaydetmek engellenmesin
            var taxNumber = await ValidateTaxNumberAsync(updateCustomerDto.CountryId, updateCustomerDto.CustomerType,
                                                         updateCustomerDto.TaxNumber, excludeCustomerId: customer.Id);

            customer.Title = updateCustomerDto.Title.Trim();
            customer.CustomerType = updateCustomerDto.CustomerType;
            customer.CountryId = updateCustomerDto.CountryId;
            customer.TaxNumber = taxNumber;
            customer.TaxOffice = string.IsNullOrWhiteSpace(updateCustomerDto.TaxOffice) ? "-" : updateCustomerDto.TaxOffice.Trim();
            customer.Email = updateCustomerDto.Email.Trim();
            customer.PhoneNumber = updateCustomerDto.PhoneNumber.Trim();

            // Code ve Status BİLEREK yazılmıyor: kod üretildiği gibi kalır, durum yalnız
            // SetStatusAsync'ten geçer. Formdan gelen değerlerine güvenilmiyor (D114 · D115)
            await _customerRepository.UpdateAsync(customer);
        }

        public async Task SetStatusAsync(Guid id, AccountStatus status)
        {
            var customer = await _customerRepository.GetByIdAsync(id)
                           ?? throw new ValidationException(_localizer["CustomerNotFound"].Value);

            if (customer.Status == status)
            {
                // İki sekmeden aynı düğmeye basılmış olabilir: "zaten aktif" demek yanıltıcı olur
                return;
            }

            var hedefler = AllowedTransitions.TryGetValue(customer.Status, out var izinli) ? izinli : [];
            if (!hedefler.Contains(status))
            {
                throw new ValidationException(_localizer["CustomerStatusTransitionInvalid",
                    _localizer["AccountStatus_" + customer.Status].Value,
                    _localizer["AccountStatus_" + status].Value].Value);
            }

            customer.Status = status;

            // bağlı kullanıcı hesaplarına DOKUNULMUYOR: askıdaki şirketin çalışanı
            // panele girmeye devam eder, yalnız gişede o müşteri seçilemez
            await _customerRepository.UpdateAsync(customer);
        }

        // Ülkeye göre biçim + aynı ülkede tekillik. Oluşturma ve düzenleme AYNI kuralı kullanır;
        // excludeCustomerId düzenlemede kaydın kendisini çakışma saymamak için
        private async Task<string> ValidateTaxNumberAsync(Guid countryId, CustomerType customerType,
                                                          string taxNumber, Guid? excludeCustomerId)
        {
            var country = await _countryRepository.GetByIdAsync(countryId);
            if (country == null)
            {
                throw new ValidationException(_localizer["CountryNotFound"].Value);
            }

            // "123.456.789-09" → "12345678909" · boşluk, nokta, tire atılır, harf büyütülür
            var normalized = TaxIdentityRules.Normalize(taxNumber);

            var formatErrorKey = TaxIdentityRules.GetFormatErrorKey(country.IsoCode, customerType, normalized);
            if (formatErrorKey != null)
            {
                throw new ValidationException(_localizer[formatErrorKey].Value);
            }

            if (await _customerRepository.TaxNumberExistsAsync(countryId, normalized, excludeCustomerId))
            {
                throw new ValidationException(_localizer["TaxNumberTaken"].Value);
            }

            return normalized;
        }
    }
}
