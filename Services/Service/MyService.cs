public class MyService
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }

    public void DoSomething()
    {
        _logger.LogInformation("İşlem başlatıldı.");
        _logger.LogWarning("Bu bir uyarıdır.");
        _logger.LogError("Bir hata oluştu.");
    }
}
