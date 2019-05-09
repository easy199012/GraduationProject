﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MyService;
using UnityEngine.UI;
using Entitas;
using UnityEngine.EventSystems;

namespace UI
{
    public enum ButtonCanvasEnum
    {
        Equip, Skill, Item, Setting
    }
    public class SelectionView : IUserInterface
    {
        //5个物品选择框的回调
        private GameObject SelectItem = null;
        private GameObject SelectItem2 = null;
        private GameObject SelectSkill = null;
        private GameObject SelectSkill2 = null;


        //暂存打开背包的创建的物品，在关闭时要摧毁。
        private List<GameObject> Items = new List<GameObject>();


        ButtonCanvasEnum CurrentCanvas = ButtonCanvasEnum.Equip;

        [Header("Root")]
        public GameObject Root;
        [Header("UI相机")]
        public GameObject Uicamera; 
        //根画布下
        [Header("Root Canvas")]
        public Button QuitButton;
        public Button EquipButton;
        public Button ItemButtom;
        public Button SkillButton;
        public Button SettingButton;

        //装备按钮下
        [Header("Equip Button")]
        public Canvas EquipCanvas;
        public Canvas SelectItemCanvas;
        public Canvas SelectSkillCanvas;

        //装备按钮下的装备画布
        [Header("Equip Button/Equip Canvas")]
        public Button[] ItemMenu = new Button[5];
        public Button SkillMenu;
        public Text Describe;
        //装备按钮下的选择物品画布
        [Header("Equip Button/SelectItem Canvas")]
        public Text SelectItemDescribe;
        public Button SelectItemQuit;
        public GridLayoutGroup GridLayoutGroup;
        public GameObject SelectItemContent;
        //装备按钮下的选择技能画布
        [Header("Equip Button/SelectSkill Canvas")]
        public Text SelectSkillDescribe;
        public Button SelectSkillQuit;
        public GameObject SelectSkillContent;

        //设置按钮下
        [Header("Setting Button")]
        public Canvas SettingCanvas;
        //设置按钮下的SettingCanvas
        [Header("Setting Button/Setting Canvas")]
        public Button GameOption;
        public Button QuitGame;
        public Button SaveAndQuit;


        

        private void Awake()
        {


            //初始化设置
            SetgameobjectActive(EquipCanvas.gameObject, true);
            SetgameobjectActive(SelectItemCanvas.gameObject, false);
            SetgameobjectActive(SelectItemCanvas.gameObject, false);
            SetgameobjectActive(SelectSkillCanvas.gameObject, false);
            SetgameobjectActive(SettingCanvas.gameObject, false);

            //初始化设置回调函数
            //给顶端按钮设置回调
            SetButtonCallback(QuitButton, QuitEvent);
            SetButtonCallback(EquipButton, EquipEvent);
            SetButtonCallback(SettingButton, SettingEvent);

            //物品选择界面的回调
            //5个物品选择框的回调
            //int[] IndexinPack = new int[5];
            foreach (var item in ItemMenu)
            {
                SetButtonCallback(item, ItemSelectEvent);
            }
            //物品选择的确定按钮
            SetButtonCallback(SelectItemQuit, SelectItemQuitEvent);

            //技能选择界面的回调
            SetButtonCallback(SkillMenu, SkillSelectEvent);
            SetButtonCallback(SelectSkillQuit, SelectSkillQuitEvent);

            //设置界面的回调
            SetButtonCallback(QuitGame, QuitGameEvent);
            SetButtonCallback(SaveAndQuit, SaveAndQuitEvent);
            SetButtonCallback(GameOption, GameOptionEvent);
        }

        private void Start()
        {
            //找到UI相机
            //Uicamera =  GameObject.Find("UICamera");
            //if (Uicamera == null)
            //{
            //    Debug.LogError("找不到UI相机");
            //}
            //else
            //{
            //    Root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            //    Root.GetComponent<Canvas>().
            //}
            MyEventSystem.Instance.Subscribe(DescribeObjectArgs.Id, ShowItemDescribe);
            MyEventSystem.Instance.Subscribe(UpdateItemIconArgs.Id, UpdateItemsIconEvent);
        }



        private void OnDestroy()
        {
            MyEventSystem.Instance.UnSubscribe(DescribeObjectArgs.Id, ShowItemDescribe);
            MyEventSystem.Instance.UnSubscribe(UpdateItemIconArgs.Id, UpdateItemsIconEvent);
        }

        //显示物品描述
        private void ShowItemDescribe(object sender, GameEventArgs e)
        {
            Dictionary<int, Dictionary<Item, int>> m_backpack = PlayerController.Instance.GetBackPackDate();
            DescribeObjectArgs args = e as DescribeObjectArgs;
            int index2 = FindItemAtPack(args.GameObject, Items);
            if (index2 == -1)
            {
                Debug.LogError("在背包里没有找到");
                return;
            }
            Item item = m_backpack.ElementAt(index2).Value.First().Key;
            SelectItemDescribe.text = item.Describe;
            Describe.text = item.Describe;
        }
        //在使用物品后更新5物品栏，这是回调函数
        private void UpdateItemsIconEvent(object sender, GameEventArgs e)
        {
            UpdateItemButton();
        }

        private void GameOptionEvent()
        {
            UIService.Instance.PushView(GameConfigService.Instance.UIPrefabPath + "Tips");
        }

        private void SaveAndQuitEvent()
        {
            Application.Quit();
        }

        private void QuitGameEvent()
        {
            Application.Quit();
        }

        //选择物品按钮回调
        void SelectItemEvent()
        {
        }
        //选这个技能按钮的回调
        void SelectSkillEvent()
        {

        }
        //选择技能界面的退出按钮
        private void SelectSkillQuitEvent()
        {
            UIService.Instance.CloseNotifyWithinCanvas(SelectSkillCanvas.gameObject);
        }
        //选择物品界面的退出按键
        private void SelectItemQuitEvent()
        {
            UIService.Instance.CloseNotifyWithinCanvas(SelectItemCanvas.gameObject);
            DestroyItemButton();

        }
        //装备界面点击选择技能
        private void SkillSelectEvent()
        {
            UIService.Instance.OpneNotifyWithinCanvas(SelectSkillCanvas.gameObject);
        }
        //装备界面点击选择物品
        void ItemSelectEvent()
        {
            SelectItem = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            UIService.Instance.OpneNotifyWithinCanvas(SelectItemCanvas.gameObject);
            CreateItemButton();
        }
        //在选择物品里面选择物品完成后的回调
        private void SelectedItem()
        {
            Dictionary<int, Dictionary<Item, int>> m_backpack = PlayerController.Instance.GetBackPackDate();
            Item[] m_CurrentItems = PlayerController.Instance.GetCurrentPackDate();

            SelectItem2 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            int index1 = FindItemAtPack(SelectItem, ItemMenu);
            int index2 = FindItemAtPack(SelectItem2, Items);
            if (index2 == -1)
            {
                Debug.LogError("在背包里没有找到");
                return;
            }
            UIService.Instance.CloseNotifyWithinCanvas(SelectItemCanvas.gameObject);
            Item item = m_backpack.ElementAt(index2).Value.First().Key;
            Describe.text = item.Describe;

            int tmp = IsContainType(item, m_CurrentItems);
            if (tmp != -1)
            {
                if (tmp == index1)
                {
                    m_CurrentItems[tmp] = null;
                }
                else
                {
                    m_CurrentItems[tmp] = null;
                    m_CurrentItems[index1] = item;
                }
            }
            else
            {
                m_CurrentItems[index1] = item;
            }

            UpdateItemButton();
            DestroyItemButton();

        }
        int IsContainType(Item TargetItem,Item[] items)
        {
            for (int i = 0; i < items.Count(); i++)
            {
                if (items[i] == null)
                {
                    continue;
                }
                if (items[i].GetType() == TargetItem.GetType())
                {
                    return i;
                }
            }
            return -1;
        }
        int FindItemAtPack(GameObject TargetObject,List<GameObject> gameObjects)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i] == TargetObject)
                {
                    return i;
                }
            }
            return -1;
        }
        int FindItemAtPack(GameObject TargetObject, Button[] buttons)
        {
            for (int i = 0; i < buttons.Count(); i++)
            {
                if (buttons[i] == null)
                {
                    continue;
                }
                if (buttons[i].gameObject == TargetObject)
                {
                    return i;
                }
            }
            return -1;
        }
        void SettingEvent()
        {
            UIService.Instance.ChangeNotifyWithinCanvas(GetCurrentCanvas(), SettingCanvas.gameObject);
            CurrentCanvas = ButtonCanvasEnum.Setting;

        }

        void EquipEvent()
        {
            UpdateItemButton();
            UIService.Instance.ChangeNotifyWithinCanvas(GetCurrentCanvas(), EquipCanvas.gameObject);
            CurrentCanvas = ButtonCanvasEnum.Equip;
        }

        void QuitEvent()
        {
            //通知更新UI图标
            MyEventSystem.Instance.Invoke(UpdateItemIconArgs.Id, this, new UpdateItemIconArgs() { });
            //切换场景
            MyEventSystem.Instance.Invoke(ChangeGameStateArgs.Id, this, new ChangeGameStateArgs() { SceneEnum = SceneEnum.Battle });

        }

        //获取当前的的Canvas，在改变canvas的时候把当前关闭，打开需要打开的
        public GameObject GetCurrentCanvas()
        {
            switch (CurrentCanvas)
            {
                case ButtonCanvasEnum.Equip:
                    return EquipCanvas.gameObject;
                case ButtonCanvasEnum.Skill:

                case ButtonCanvasEnum.Item:
                    return null;
                case ButtonCanvasEnum.Setting:
                    return SettingCanvas.gameObject;
                default:
                    return null;
            }
        }

        //在物品选择界面创建物品
        private void CreateItemButton()
        {
            int m_count = PlayerController.Instance.GetCountFromBack();
            Dictionary<int, Dictionary<Item, int>> m_backpack = PlayerController.Instance.GetBackPackDate();

            foreach (var item in m_backpack)
            {
                Button button;
                button = CreatButton(SelectItemContent.transform);
                button.GetComponentInChildren<Text>().text = item.Value.Keys.First().Name + "     x" + item.Value.Values.First().ToString();

                SetButtonMouseXXXCallback(button, ShowItemDescribe);
                SetButtonCallback(button, SelectedItem);

                Items.Add(button.gameObject);
            }
        }



        //鼠标滑入事件
        private void ShowItemDescribe(BaseEventData arg0)
        {
            Dictionary<int, Dictionary<Item, int>> m_backpack = PlayerController.Instance.GetBackPackDate();
            //利用射线
            //GameObject gameobj=null ;
            //Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit, LayerMask.GetMask("UI")));
            //{
            //    Debug.DrawLine(ray.origin, hit.point);
            //    gameobj = hit.collider.gameObject;
            //}
            //if (gameobj !=null)
            //{


            //利用UI摄像获取Button
            //PointerEventData data = new PointerEventData(EventSystem.current);
            //data.position = Input.mousePosition;
            //data.pressPosition = Input.mousePosition;
            //List< RaycastResult> results = new List<RaycastResult>();
            //Root.GetComponent<GraphicRaycaster>().Raycast(data, results);

            //if (results.Count > 0)
            //{
            //    for (int i = 0; i < results.Count; i++)
            //    {
            //        Debug.Log(results[i].gameObject.name);
            //    }
            //}
            //else
            //{
            //    Debug.Log("没有UI");
            //}


            //设置Describe
            //int index2 = FindItemAtPack(EventSystem.current.gameObject, Items);
            //if (index2 == -1)
            //{
            //    Debug.LogError("在背包里没有找到");
            //    return;
            //}
            //Item item = m_backpack.ElementAt(index2).Value.First().Key;
            //    SelectItemDescribe.text = item.Describe;
            //}
            
        }

        void OnPointEnter(BaseEventData eventData)
        {

        }

        //摧毁物品选择界面的物品，在每次退出后要摧毁
        private void DestroyItemButton()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == null)
                {
                    continue;
                }
                GameObject.Destroy(Items[i].gameObject);
            }
            Items.Clear();
        }


        //事件通知UI更新，更新5个物品的UI
        private void UpdateItemButton()
        {
            string Name;
            int m_count = PlayerController.Instance.GetCountFromBack();
            Dictionary<int, Dictionary<Item, int>> m_backpack = PlayerController.Instance.GetBackPackDate();
            Item[] m_CurrentItems = PlayerController.Instance.GetCurrentPackDate();

            for (int i = 0; i < m_CurrentItems.Count(); i++)
            {
                if (m_CurrentItems[i] == null)
                {
                    Name = Item.DefaultIconName;
                    ItemMenu[i].GetComponent<Image>().sprite = ResourceService.Instance.LoadAsset<Sprite>(GameConfigService.Instance.UIIcon + Name);
                    continue;
                }
                else
                {
                    Name = m_CurrentItems[i].IconName;

                    //SelectItem.GetComponent<Image>().sprite = item.GetItemIcon(Name);
                    ItemMenu[i].GetComponent<Image>().sprite = m_CurrentItems[i].GetItemIcon(Name);
                }
            }
        }


        //创建物品按钮
        private Button CreatButton(Transform Parent)
        {
            Button button;
            button = ResourceService.Instance.InstantiateAsset<Button>(GameConfigService.Instance.UIPrefabPath + "Button");
            button.transform.SetParent(Parent);
            Debug.Log(button.gameObject.GetComponent<RectTransform>().localScale);
            //buttons[i].gameObject.GetComponent<RectTransform>().localScale.Scale(new Vector3(1, 1, 1));
            //buttons[i].gameObject.GetComponent<RectTransform>().localScale.Set(1, 1, 1);
            button.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            Vector3 vector3 = button.gameObject.GetComponent<RectTransform>().localPosition;
            vector3.z = 0;
            button.gameObject.GetComponent<RectTransform>().localPosition = vector3;

            if (button.gameObject.GetComponent<Collider2D>() == null)
            {
                button.gameObject.AddComponent<Collider2D>();
            }
            return button;
        }
        //给按钮添加鼠标滑入事件

    }
}
