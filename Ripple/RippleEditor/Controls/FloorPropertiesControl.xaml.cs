using RippleDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RippleEditor.Controls
{
    /// <summary>
    /// Interaction logic for FloorPropertiesControl.xaml
    /// </summary>
    public partial class FloorPropertiesControl : UserControl
    {
        public FloorPropertiesControl()
        {
            InitializeComponent();
            InitializeControls();
        }

        #region Common Control Methods
        public void InitializeControls()
        {
            try
            {
                //Populate the tile types - default text
                this.CBTypeValue.Items.Clear();
                foreach (var typeName in Enum.GetNames(typeof(RippleDictionary.TileType)))
                {
                    this.CBTypeValue.Items.Add(typeName);
                }
                this.CBTypeValue.SelectedValue = RippleDictionary.TileType.Text.ToString();

                //Populate the Action types - default standard
                this.CBActionValue.Items.Clear();
                foreach (var actionName in Enum.GetNames(typeof(TileAction)))
                {
                    this.CBActionValue.Items.Add(actionName);
                }
                this.CBActionValue.SelectedValue = TileAction.Standard.ToString();

                //Initialize the text boxes
                this.ActionURIBrowseButton.IsEnabled = false;
                this.clrPicker.SelectedColor = (Color)ColorConverter.ConvertFromString("#FFFFFFFF");
                this.TBTextValue.Text = "";
                this.TBActionURIValue.Text = "";
                this.TBContentValue.Text = "";
                this.TBActionURIValue.ToolTip = "";
                this.TBContentValue.ToolTip = "";
                this.ContentBrowseButton.IsEnabled = false;
                this.TBActionURIValue.IsReadOnly = true;
                this.TBActionURIValue.IsReadOnlyCaretVisible = true;
                this.TBActionURIValue.Text = "";
                this.TBActionURIValue.ToolTip = "";
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in InitializeControls for Floor Properties: {0}", ex.Message);
            }
        }

        private bool ValidateTextValue(object sender)
        {
            TextBox tb = sender as TextBox;
            if (!String.IsNullOrEmpty(tb.Text))
            {
                if (tb.Text.Length > Utilities.Constants.MaxCharForTileName)
                {
                    MessageBox.Show(String.Format("The name for the tile cannot exceed {0} characters", Utilities.Constants.MaxCharForTileName));
                    tb.Text = "";
                    return false;
                }
            }
            return true;
        }

        public bool ValidateControl()
        {
            try
            {
                //Text value can be empty, or any value less than Utilities.Constants.MaxCharValue
                if (!ValidateTextValue(this.TBTextValue))
                    return false;

                //Color value can be any value right now

                //Combo box for Type
                //Content is mandatory for anything except Text and blank
                if (!this.CBTypeValue.SelectedValue.Equals(RippleDictionary.TileType.Text.ToString()))
                {
                    if (String.IsNullOrEmpty(this.TBContentValue.Text))
                    {
                        MessageBox.Show("Please select a valid URI for Tile Content in Floor Properties");
                        return false;
                    }
                }

                //Combo box for Action
                //Content is mandatory for Animation and QRCode
                if (this.CBActionValue.SelectedValue.ToString().Equals(TileAction.HTML.ToString()) || this.CBActionValue.SelectedValue.Equals(TileAction.QRCode.ToString()))
                {
                    if (String.IsNullOrEmpty(this.TBActionURIValue.Text))
                    {
                        MessageBox.Show("Please select a valid URI for Tile Action Content in Floor Properties");
                        return false;
                    }

                    //Validate the content value
                    String actionContent = this.TBActionURIValue.Text;

                    //Animation - local and web
                    if(this.CBActionValue.SelectedValue.ToString().Equals(TileAction.HTML.ToString()))
                    {
                        if (!actionContent.StartsWith("http") && (!(actionContent.EndsWith(".htm") || actionContent.EndsWith(".html"))))
                        {
                            MessageBox.Show("Please enter valid value for HTML based URI, it should have an extension html or htm for local files, for web hosted files it should start with http");
                            return false;
                        }
                    }

                    //QRCode - web urls only
                    if (this.CBActionValue.SelectedValue.ToString().Equals(TileAction.QRCode.ToString()))
                    {
                        if (!actionContent.StartsWith("http"))
                        {
                            MessageBox.Show("Please enter valid value for HTML based URI, it should start with http");
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ValidateControl for Floor Properties: {0}", ex.Message);
                return false;
            }
        }

        public void SaveFloorProperties(RippleDictionary.Tile tile)
        {
            try
            {
                //Name
                tile.Name = this.TBTextValue.Text;
                //Tile type
                tile.TileType = (RippleDictionary.TileType)this.CBTypeValue.SelectedIndex;
                //Content
                tile.Content = this.TBContentValue.Text;
                //Color
                tile.Color = this.clrPicker.SelectedColor;
                //Action Type
                tile.Action = (TileAction)this.CBActionValue.SelectedIndex;
                //Action Content
                tile.ActionURI = this.TBActionURIValue.Text;
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in SaveFloorProperties for Floor Properties: {0}", ex.Message);
            }
        }

        public void SetFloorProperties(RippleDictionary.Tile tile)
        {
            try
            {
                //Text
                this.TBTextValue.Text = tile.Name;
            
                //Color
                this.clrPicker.SelectedColor = tile.Color;

                //Tile Type
                this.CBTypeValue.SelectedValue = tile.TileType.ToString();

                //Tile content
                if ((!String.IsNullOrEmpty(tile.Content)) && (tile.Content.StartsWith(@"\Assets\")))
                    this.TBContentValue.Text = Utilities.HelperMethods.TargetAssetsRoot + tile.Content;
                else
                    this.TBContentValue.Text = tile.Content;

                //Action type
                this.CBActionValue.SelectedValue = tile.Action.ToString();                

                //Action content
                if ((!String.IsNullOrEmpty(tile.ActionURI)) && (tile.ActionURI.StartsWith(@"\Assets\")))
                    this.TBActionURIValue.Text = Utilities.HelperMethods.TargetAssetsRoot + tile.ActionURI;
                else
                    this.TBActionURIValue.Text = tile.ActionURI;

                //UI settings
                //Content browse button
                if (CBTypeValue.SelectedValue.ToString().Equals(RippleDictionary.TileType.Text.ToString()))
                {
                    this.ContentBrowseButton.IsEnabled = false;
                }
                else
                {
                    this.ContentBrowseButton.IsEnabled = true;
                }

                //Action URI browse button and textbox
                if (CBActionValue.SelectedValue.ToString().Equals(TileAction.HTML.ToString()))
                {
                    this.ActionURIBrowseButton.IsEnabled = true;
                    this.TBActionURIValue.IsReadOnly = true;
                    this.TBActionURIValue.IsReadOnlyCaretVisible = true;
                }
                else if (CBActionValue.SelectedValue.ToString().Equals(TileAction.QRCode.ToString()))
                {
                    this.ActionURIBrowseButton.IsEnabled = false;
                    this.TBActionURIValue.IsReadOnly = false;
                    this.TBActionURIValue.IsReadOnlyCaretVisible = false;
                }
                else
                {
                    this.ActionURIBrowseButton.IsEnabled = false;
                    this.TBActionURIValue.IsReadOnly = true;
                    this.TBActionURIValue.IsReadOnlyCaretVisible = true;
                }

                //Custom disablement for start
                if (tile.Id.Equals("Tile0"))
                {
                    this.TBTextValue.IsEnabled = false;
                    this.CBTypeValue.IsEnabled = false;
                    this.CBActionValue.IsEnabled = false;
                }
                else
                {
                    this.TBTextValue.IsEnabled = true;
                    this.CBTypeValue.IsEnabled = true;
                    this.CBActionValue.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in SetFloorProperties for Floor Properties: {0}", ex.Message);                
            }
        } 
        #endregion

        #region UI Methods
        private void CBTypeValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cb = sender as ComboBox;
                if (cb.SelectedValue != null)
                {
                    if (cb.SelectedValue.ToString().Equals(RippleDictionary.TileType.Text.ToString()))
                    {
                        this.ContentBrowseButton.IsEnabled = false;
                    }
                    else
                    {
                        this.ContentBrowseButton.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in CBTypeValue_SelectionChanged for Floor Properties: {0}", ex.Message);
            }
        }

        private void CBActionValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cb = sender as ComboBox;
                if (cb.SelectedValue != null)
                {
                    if (cb.SelectedValue.ToString().Equals(TileAction.HTML.ToString()))
                    {
                        this.ActionURIBrowseButton.IsEnabled = true;
                        this.TBActionURIValue.IsReadOnlyCaretVisible = false;
                        this.TBActionURIValue.IsReadOnly = false;
                        this.TBActionURIValue.Text = "";
                        this.TBActionURIValue.ToolTip = "";
                    }
                    else if (cb.SelectedValue.ToString().Equals(TileAction.QRCode.ToString()))
                    {
                        this.ActionURIBrowseButton.IsEnabled = false;
                        this.TBActionURIValue.IsReadOnlyCaretVisible = false;
                        this.TBActionURIValue.IsReadOnly = false;
                        this.TBActionURIValue.Text = "";
                        this.TBActionURIValue.ToolTip = "";
                    }
                    else
                    {
                        this.ActionURIBrowseButton.IsEnabled = false;
                        this.TBActionURIValue.IsReadOnly = true;
                        this.TBActionURIValue.IsReadOnlyCaretVisible = true;
                        this.TBActionURIValue.Text = "";
                        this.TBActionURIValue.ToolTip = "";
                    }
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in CBActionValue_SelectionChanged for Floor Properties: {0}", ex.Message);
            }
        }

        private void ActionURIBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Show the dialog box only if Animation mode
                if (this.CBActionValue.SelectedValue.ToString() == TileAction.HTML.ToString())
                {
                    System.Windows.Forms.OpenFileDialog dlgBox = new System.Windows.Forms.OpenFileDialog();
                    dlgBox.Filter = "Animation files(*.swf;*.html;*.htm;)|*.swf;*.html;*.htm;";
                    var res = dlgBox.ShowDialog();
                    if (res == System.Windows.Forms.DialogResult.OK)
                    {
                        //Get the complete fileName
                        String updatedFileName = dlgBox.FileName;
                        if (System.IO.Path.GetExtension(updatedFileName).ToLower().Equals(".swf"))
                        {
                            updatedFileName = Utilities.HelperMethods.CopyFile(dlgBox.FileName, Utilities.HelperMethods.TargetAssetsDirectory + "\\Animations");
                        }
                        else
                        {
                            String targetfolder = Utilities.HelperMethods.CopyFolder(System.IO.Path.GetDirectoryName(updatedFileName), Utilities.HelperMethods.TargetAssetsDirectory + "\\Animations");
                            updatedFileName = targetfolder + "\\" + System.IO.Path.GetFileName(updatedFileName);
                        }
                        if (!String.IsNullOrEmpty(updatedFileName))
                        {
                            this.TBActionURIValue.Text = updatedFileName;
                            this.TBActionURIValue.ToolTip = updatedFileName;
                        }
                        else
                        {
                            this.TBActionURIValue.Text = "";
                            this.TBActionURIValue.ToolTip = "";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Browse applicable only for Action = Animation");
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ActionURIBrowseButton_Click for Floor Properties: {0}", ex.Message);
            }
        }

        private void ContentBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Applicable only for types other than text
                if (!(this.CBTypeValue.SelectedValue.ToString() == RippleDictionary.TileType.Text.ToString()))
                {
                    System.Windows.Forms.OpenFileDialog dlgBox = new System.Windows.Forms.OpenFileDialog();

                    if (this.CBTypeValue.SelectedValue.ToString() == RippleDictionary.TileType.OnlyMedia.ToString())
                        dlgBox.Filter = "Media Files(*.mp4;*.wmv;*.jpeg;*.png;*.jpg;*.bmp;)|*.mp4;*.wmv;*.jpeg;*.png;*.jpg;*.bmp;|Videos(*.mp4;*.wmv;)|*.mp4;*.wmv;|Images(*.jpeg;*.png;*.jpg;*.bmp;)|*.jpeg;*.png;*.jpg;*.bmp;";
                    else
                        dlgBox.Filter = "Images(*.jpeg;*.png;*.jpg;*.bmp;)|*.jpeg;*.png;*.jpg;*.bmp;";

                    var res = dlgBox.ShowDialog();
                    if (res == System.Windows.Forms.DialogResult.OK)
                    {
                        String targetFolder = Utilities.HelperMethods.TargetAssetsDirectory;
                        String fileExt = System.IO.Path.GetExtension(dlgBox.FileName).ToLower();
                        if (fileExt.Equals(".mp4") || fileExt.Equals(".wmv"))
                            targetFolder += "\\Videos";
                        else
                            targetFolder += "\\Images";
                        //Get the complete fileName
                        String updatedFileName = Utilities.HelperMethods.CopyFile(dlgBox.FileName, targetFolder);
                        if (!String.IsNullOrEmpty(updatedFileName))
                        {
                            this.TBContentValue.Text = updatedFileName;
                            this.TBContentValue.ToolTip = updatedFileName;
                        }
                        else
                        {
                            this.TBContentValue.Text = "";
                            this.TBContentValue.ToolTip = "";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Browse/Content not applicable for TileType = Text");
                }
            }
            catch (Exception ex)
            {

                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ContentBrowseButton_Click for Floor Properties: {0}", ex.Message);
            }
        }

        private void TBTextValue_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateTextValue(sender);
        }
        #endregion
    }
}
