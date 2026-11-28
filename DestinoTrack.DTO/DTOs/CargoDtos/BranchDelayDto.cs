namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class BranchDelayDto
    {
        public string BranchName { get; set; } = string.Empty;

        // Şubeye VARIŞ kargosu içinde geciken sayısı. BranchTrafficDto.Count ile
        // karıştırılmamalı: o çıkışı da sayar, bu yalnız varışı — gecikme varış taahhüdünün ihlali
        public int DelayedCount { get; set; }
    }
}
