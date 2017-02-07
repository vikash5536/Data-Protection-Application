//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace DataProtectionApplication.TaskSchedulingApp.UserControls
{
    /// <summary>
    /// Interaction logic for MultiSelectionComboWithoutAll.xaml
    /// </summary>
    public partial class MultiSelectionComboWithoutAll : UserControl
    {
        private ObservableCollection<Node> _nodeList;
        public MultiSelectionComboWithoutAll()
        {
            InitializeComponent();
            _nodeList = new ObservableCollection<Node>();
        }
        #region Dependency Properties

        public static readonly DependencyProperty ItemsSourceProperty =
             DependencyProperty.Register("ItemsSource", typeof(Dictionary<string, object>), typeof(MultiSelectionComboWithoutAll), new FrameworkPropertyMetadata(null,
        new PropertyChangedCallback(MultiSelectionComboWithoutAll.OnItemsSourceChanged)));

        public static readonly DependencyProperty SelectedItemsProperty =
         DependencyProperty.Register("SelectedItems", typeof(Dictionary<string, object>), typeof(MultiSelectionComboWithoutAll), new FrameworkPropertyMetadata(null,
     new PropertyChangedCallback(MultiSelectionComboWithoutAll.OnSelectedItemsChanged)));

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectionComboWithoutAll), new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText", typeof(string), typeof(MultiSelectionComboWithoutAll), new UIPropertyMetadata(string.Empty));



        public Dictionary<string, object> ItemsSource
        {
            get { return (Dictionary<string, object>)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public Dictionary<string, object> SelectedItems
        {
            get { return (Dictionary<string, object>)GetValue(SelectedItemsProperty); }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }
        #endregion

        #region Events
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectionComboWithoutAll control = (MultiSelectionComboWithoutAll)d;
            control.DisplayInControl();
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectionComboWithoutAll control = (MultiSelectionComboWithoutAll)d;
            control.SelectNodes();
            control.SetText();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox clickedBox = (CheckBox)sender;



            int _selectedCount = 0;
            foreach (Node s in _nodeList)
            {
                if (s.IsSelected && s.Title != "All")
                    _selectedCount++;
            }

            SetSelectedItems();
            SetText();

        }
        #endregion


        #region Methods
        private void SelectNodes()
        {
            foreach (KeyValuePair<string, object> keyValue in SelectedItems)
            {
                Node node = _nodeList.FirstOrDefault(i => i.Title == keyValue.Key);
                if (node != null)
                    node.IsSelected = true;
            }
        }

        private void SetSelectedItems()
        {
            if (SelectedItems == null)
                SelectedItems = new Dictionary<string, object>();
            SelectedItems.Clear();
            foreach (Node node in _nodeList)
            {
                if (node.IsSelected && node.Title != "All")
                {
                    if (this.ItemsSource.Count > 0)

                        SelectedItems.Add(node.Title, this.ItemsSource[node.Title]);
                }
            }
        }

        private void DisplayInControl()
        {
            _nodeList.Clear();
            foreach (KeyValuePair<string, object> keyValue in this.ItemsSource)
            {
                Node node = new Node(keyValue.Key);
                _nodeList.Add(node);
            }
            MultiSelectComboWithoutAll.ItemsSource = _nodeList;
        }

        private void SetText()
        {
            if (this.SelectedItems != null)
            {
                StringBuilder displayText = new StringBuilder();
                foreach (Node s in _nodeList)
                {
                    if (s.IsSelected == true && s.Title != "All")
                    {
                        displayText.Append(s.Title);
                        displayText.Append(',');
                    }
                }
                this.Text = displayText.ToString().TrimEnd(new char[] { ',' });
            }
            // set DefaultText if nothing else selected
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Text = this.DefaultText;
            }
        }


        #endregion
    }    
}
