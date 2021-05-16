using System;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class GachaStoreItemComponent : MonoBehaviour
    {
        [SerializeField] Image _image;
        [SerializeField] TextMeshProUGUI _nameText;
        [SerializeField] TextMeshProUGUI _descriptionText;
        [SerializeField] Button _button;

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <param name="storeItem"></param>
        public void Initialize(StoreItem storeItem)
        {
            var item = CatalogManager.CatalogItems[storeItem.ItemId];
            _nameText.text = item.DisplayName;
            _descriptionText.text = item.Description;
            _button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => PurchaseAsync(storeItem).Forget());
        }

        /// <summary>
        /// アイテムを購入する。
        /// </summary>
        /// <param name="storeItem"></param>
        /// <returns></returns>
        private async UniTaskVoid PurchaseAsync(StoreItem storeItem)
        {
            var vc = VirtualCurrencyNames.MS;
            var price = storeItem.VirtualCurrencyPrices[vc.Code];
            if (VirtualCurrencyManager.MagicStone < price)
            {
                await MessageDialog.ShowAsync($"{vc.Name}が不足しています。");
                return;
            }

            var buyItem = CatalogManager.CatalogItems[storeItem.ItemId];
            var confirm = await ConfirmDialog.ShowAsync($"{vc.Name} {price} 個で{buyItem.DisplayName}を購入しますか？");
            if (confirm == DialogResult.Cancel)
                return;

            var (result, _) = await UniTask.WhenAll(
                StoreManager.PurchaseItemAsync(StoreId.GachaStore, storeItem.ItemId, vc.Code),
                MessageDialog.ShowAsync("ガチャ演出再生中").AsAsyncUnitUniTask());

            var getItems = result.Items.Where(x => x.BundleParent != null);
            foreach (var item in getItems)
            {
                await MessageDialog.ShowAsync($"{item.DisplayName}を獲得しました");
            }
        }
    }
}
