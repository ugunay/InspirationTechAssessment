namespace InspirationTechAssessment.Interfaces
{
    public interface IPaymentStrategy
    {
        decimal CalculateTotalAmount(decimal amount);
    }
}