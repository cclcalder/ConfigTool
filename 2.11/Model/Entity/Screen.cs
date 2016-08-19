using System;
using System.Collections.Generic;
using System.ComponentModel;
using Model.Annotations;

namespace Model.Entity
{
     //<Screen>
     //   <Key>TERMS</Key>
     //   <Label>Terms</Label>
     //   <LabelKey>TERMS</LabelKey>
     //   <ROBAppType>300</ROBAppType>
     //   <ROBRecipient>0</ROBRecipient>
     //   <ShowAsROBGroup>1</ShowAsROBGroup>
     //   <IsDefault>0</IsDefault>
     //   <SortOrder>300</SortOrder>
     //   <Tabs>
     //     <Tab>
     //       <Key>TERMS</Key>
     //       <Label>TERMS</Label>
     //       <LabelKey>TAB_TERMS</LabelKey>
     //       <IsDefault>1</IsDefault>
     //       <NewButton>1</NewButton>
     //       <CopyButton>1</CopyButton>
     //       <DeleteButton>1</DeleteButton>
     //     </Tab>
     //     <Tab>
     //       <Key>TERMS</Key>
     //       <Label>Terms</Label>
     //       <IsDefault>1</IsDefault>
     //       <NewButton>1</NewButton>
     //       <CopyButton>1</CopyButton>
     //       <DeleteButton>1</DeleteButton>
     //     </Tab>
     //   </Tabs>
     // </Screen>

    public class Screen : INotifyPropertyChanged
    {
        public string Key { get; set; }

        public string ParentScreen_Key { get; set; }
        public string LabelKey { get; set; }

        public string Icon { get; set; }

        public int SortOrder { get; set; }

        /// <summary>
        /// If set to true it will add:
        ///  <Screen_Code>value_of_key</Screen_Code> 
        /// to the xml in of the get customers / listings proc
        /// </summary>
        public bool UseKeyToLoadData { get; set; }

        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value; 
                OnPropertyChanged("Label"); 
            }
        }

        public string Abbreviation
        {
            get
            { 
                var L = "";
                var r = "";

                var Lr = _label.Split(' ');
                if (Lr.Length > 1)
                {
                    L = Lr[0].Left(1).ToUpper();
                    if (Lr[1].Substring(0, 1) == "&")
                    {
                        r = Lr[2].Left(1);
                    }
                    else
                    {
                        r = Lr[1].Substring(0, 1).ToLower();
                    }

                }

                L = _label.Left(1).ToUpper();
                r = _label.Substring(1, 1).ToLower();

                return L + r;
    
            }
        }

        public bool IsDefault { get; set; }

        private string _uri;
        public string Uri
        {
            get { return _uri; }
            set
            {
                _uri = value;
                OnPropertyChanged("Uri");
            }
        }

        //public string Path
        //{
        //    get
        //    {
        //        if (String.IsNullOrEmpty(RobAppType))
        //        {
        //            return Uri;
        //        }
        //        else
        //        {
        //            //if (ShowAsROBGroup)
        //            //{
        //            //    return Uri + "?ID=" + RobAppType;
        //            //}
        //            //else
        //            //{
        //                return Uri;
        //            //}
        //        }
        //    }
        //}

        public string RobAppType { get; set; }
        public bool RobAppRecipient { get; set; }
        public bool ShowAsROBGroup { get; set; }

        public bool NewButton { get; set; }
        public bool CopyButton { get; set; }
        public bool DeleteButton { get; set; }
        public bool IsSelected { get; set; }
        public bool IsFilteredByListings { get; set; }

        public List<Screen> Tabs { get; set; }

        public static List<Screen> DEV_ONLY()
        { 
            List<Screen> screens =  new List<Screen>();

           // screens.Add(new Screen() { IsDefault = false, Key = "TermsV2", Label = "TermsV2" , Uri = "/Pages/RobGroups/GroupsList.xaml"});
            screens.Add(new Screen() { IsDefault = false, LabelKey = "Pivot", Label = "Pivot", Uri = "/Pages/canvas/dummy/Pivot1.xaml" });

            return screens;

        }

        private List<Screen> _children = new List<Screen>();
        public List<Screen> Children { get { return _children; } set { _children = value; } }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName)); 
        }
    }
}
