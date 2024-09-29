using LoyaltyPointsApi.Models;

namespace LoyaltyPointsApi.Events
{
    public delegate void NotifyEventHandler(Object sender, Promotion promotion);
    public class PromotionAddedEvent(ILogger<PromotionAddedEvent> logger)
    {
        public event NotifyEventHandler? PromotionAdded;
        public void NotifyCustomers(Promotion promotion)
        {
            OnPromotionAdded(promotion);
        }
        protected virtual void OnPromotionAdded(Promotion promotion)
        {
            if (PromotionAdded == null)
            {
                logger.LogWarning("No subscribers for PromotionAdded event.");
                return;
            }
            PromotionAdded?.Invoke(this, promotion);
        }
    }
}