using Newtonsoft.Json;
using PlayFab.ClientModels;

namespace PlayFabBook
{
    public static class ItemInstanceExtension
    {
        /// <summary>
        /// ItemInstance をプレゼントに変換する。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Present ToPresent(this ItemInstance item)
        {
            var customData = item.CustomData != null && item.CustomData.TryGetValue(ItemClass.Present.ToString(), out var json)
                ? JsonConvert.DeserializeObject<PresentCustomData>(json)
                : new PresentCustomData();

            return new Present
            {
                InstanceId = item.ItemInstanceId,
                DisplayName = item.DisplayName,
                Description = CatalogManager.CatalogItems[item.ItemId].Description,
                CustomData = customData,
            };
        }
    }
}