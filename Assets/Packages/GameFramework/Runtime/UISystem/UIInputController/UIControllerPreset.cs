using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    [CreateAssetMenu(menuName = "GameFramework/Controller Preset")]
    public class UIControllerPreset : ScriptableObject
    {
        [SerializeField]
        private string controllerName;
        [SerializeField]
        private List<UIControllerItem> controllerItems = new List<UIControllerItem>();
        private Dictionary<string, UIControllerItem> controllerItemMap = new Dictionary<string, UIControllerItem>();

        public string ControllerName
        {
            get { return controllerName; }
        }

        public void Init()
        {
            controllerItemMap.Clear();
            foreach (UIControllerItem item in controllerItems)
            {
                controllerItemMap.Add(item.itemName, item);
            }
        }

        public UIControllerItem GetControllerItem(string itemName)
        {
            controllerItemMap.TryGetValue(itemName, out UIControllerItem controllerItem);
            return controllerItem;
        }

        public void Release()
        {
            foreach (UIControllerItem item in controllerItems)
            {
                item.Release();
            }
        }
    }
}
