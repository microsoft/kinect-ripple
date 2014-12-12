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
    /// Interaction logic for ScreenPropertiesControl.xaml
    /// </summary>
    public partial class ScreenPropertiesControl : UserControl
    {
        public ScreenPropertiesControl()
        {
            InitializeComponent();
            InitializeControls();
        }

        #region Common Control methods
        public void InitializeControls()
        {
            try
            {
                //Initialize the loop video drop down
                this.LoopVideoValue.Items.Clear();
                this.LoopVideoValue.Items.Add("True");
                this.LoopVideoValue.Items.Add("False");
                this.LoopVideoValue.SelectedValue = "False";

                //Initialize the content type drop down
                this.CBContentTypeValue.Items.Clear();
                foreach (var contentType in Enum.GetNames(typeof(RippleDictionary.ContentType)))
                {
                    this.CBContentTypeValue.Items.Add(contentType);
                }
                this.CBContentTypeValue.SelectedValue = RippleDictionary.ContentType.Nothing.ToString();

                //Hide the loop video by default
                this.LoopVideoValue.Visibility = System.Windows.Visibility.Collapsed;
                this.LoopVideoLabel.Visibility = System.Windows.Visibility.Collapsed;

                //Set the header value.
                this.HeaderValue.Text = "";
                this.ContentValue.Text = "";

                this.ContentValue.IsReadOnlyCaretVisible = true;
                this.ContentValue.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in InitializeControls for Screen Properties: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Code to validate the screen properties control
        /// </summary>
        /// <returns></returns>
        public bool ValidateControl()
        {
            try
            {
                if (!ValidateHeaderValue())
                    return false;

                if (!ValidateContentValue())
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ValidateControl for Screen Properties: {0}", ex.Message);
                return false;
            }
        }

        private bool ValidateContentValue()
        {
            string currentCBContentTypeSelection = this.CBContentTypeValue.SelectedValue.ToString();
            //Empty Content for anything except nothing
            if (String.IsNullOrEmpty(this.ContentValue.Text) && (!currentCBContentTypeSelection.Equals(RippleDictionary.ContentType.Nothing.ToString())))
            {
                MessageBox.Show("Content in Screen Properties cannot be left empty unless ContentType = Nothing");
                return false;
            }

            //Verify the file extensions for browse specifically, rest cannot be altered
            String actionContent = this.ContentValue.Text;
            if (currentCBContentTypeSelection.Equals(RippleDictionary.ContentType.HTML.ToString()))
            {
                //Can take both local and web hosted URIs
                if (!actionContent.StartsWith("http") && (!(actionContent.EndsWith(".htm") || actionContent.EndsWith(".html"))))
                {
                    MessageBox.Show("Please enter valid value for HTML based URI, it should have an extension html or htm for local files, for web hosted files it should start with http");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateHeaderValue()
        {
            //Invalid header value
            if (!String.IsNullOrEmpty(this.HeaderValue.Text) && this.HeaderValue.Text.Length > Utilities.Constants.MaxCharForHeaderName)
            {
                MessageBox.Show(String.Format("Header value cannot exceed {0} characters", Utilities.Constants.MaxCharForHeaderName));
                return false;
            }
            return true;
        }

        public void SaveScreenProperties(RippleDictionary.Tile tile)
        {
            try
            {
                //Get the content Type
                RippleDictionary.ContentType ct = (RippleDictionary.ContentType)this.CBContentTypeValue.SelectedIndex;
                RippleDictionary.ScreenContent screenData = new RippleDictionary.ScreenContent(ct, tile.Id, this.HeaderValue.Text, this.ContentValue.Text, (this.LoopVideoValue.SelectedValue.ToString() == "False" ? false : true));
                MainPage.rippleData.Screen.CreateOrUpdateScreenContent(tile.Id, screenData);

                //Once successful update the corresponding screen content type for the floor tile
                Utilities.HelperMethods.GetFloorTileForID(tile.Id).CorrespondingScreenContentType = ct;
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in SaveScreenProperties for Screen Properties: {0}", ex.Message);
            }
        }

        public void SetScreenProperties(RippleDictionary.Tile tile)
        {
            try
            {
                if (MainPage.rippleData.Screen.ScreenContents.ContainsKey(tile.Id))
                {
                    //Get the screen data
                    RippleDictionary.ScreenContent screenData = MainPage.rippleData.Screen.ScreenContents[tile.Id];

                    //Set the content type
                    this.CBContentTypeValue.SelectedValue = screenData.Type.ToString();

                    //Set the loop video visibility and value
                    this.LoopVideoValue.SelectedValue = (screenData.LoopVideo == null) ? "False" : (Convert.ToBoolean(screenData.LoopVideo) ? "True" : "False");
                    if (screenData.Type == RippleDictionary.ContentType.Video)
                    {
                        this.LoopVideoLabel.Visibility = System.Windows.Visibility.Visible;
                        this.LoopVideoValue.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this.LoopVideoLabel.Visibility = System.Windows.Visibility.Collapsed;
                        this.LoopVideoValue.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    //Set the content URI
                    if ((screenData.Type == RippleDictionary.ContentType.PPT || screenData.Type == RippleDictionary.ContentType.Image || screenData.Type == RippleDictionary.ContentType.Video || screenData.Type == RippleDictionary.ContentType.HTML) && screenData.Content.StartsWith(@"\Assets\"))
                        this.ContentValue.Text = Utilities.HelperMethods.TargetAssetsRoot + screenData.Content;
                    else
                        this.ContentValue.Text = screenData.Content;

                    //Set the header text
                    this.HeaderValue.Text = screenData.Header;

                    //Set the browse button visibility
                    if (screenData.Type == RippleDictionary.ContentType.Text || screenData.Type == RippleDictionary.ContentType.HTML || screenData.Type == RippleDictionary.ContentType.Nothing)
                        this.ContentBrowseButton.IsEnabled = false;
                    else
                        this.ContentBrowseButton.IsEnabled = true;

                    //Set the content box properties
                    if (screenData.Type == RippleDictionary.ContentType.HTML || screenData.Type == RippleDictionary.ContentType.Text)
                    {
                        this.ContentValue.IsReadOnlyCaretVisible = false;
                        this.ContentValue.IsReadOnly = false;
                    }
                    else
                    {
                        this.ContentValue.IsReadOnlyCaretVisible = true;
                        this.ContentValue.IsReadOnly = true;
                    }
                }
                else
                {
                    //Just set the defaults
                    this.CBContentTypeValue.SelectedValue = RippleDictionary.ContentType.Nothing.ToString();
                    this.ContentValue.Text = "";
                    this.LoopVideoValue.SelectedValue = "False";
                    this.LoopVideoLabel.Visibility = System.Windows.Visibility.Collapsed;
                    this.LoopVideoValue.Visibility = System.Windows.Visibility.Collapsed;
                    this.HeaderValue.Text = "";
                    this.ContentBrowseButton.IsEnabled = false;
                    this.ContentValue.IsReadOnlyCaretVisible = true;
                    this.ContentValue.IsReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in SetScreenProperties for Screen Properties: {0}", ex.Message);
            }
        } 
        #endregion

        #region UI Methods
        private void ContentBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool localHTMLFiles = false;
                //Show the open file dialog box to select the content only if the content =! nothing
                string currentCBContentTypeSelection = this.CBContentTypeValue.SelectedValue.ToString();
                if (!currentCBContentTypeSelection.Equals(RippleDictionary.ContentType.Nothing.ToString()))
                {
                    System.Windows.Forms.OpenFileDialog dlgBox = new System.Windows.Forms.OpenFileDialog();

                    //Set the filter
                    if (currentCBContentTypeSelection == RippleDictionary.ContentType.Image.ToString())
                        dlgBox.Filter = "Images(*.jpeg;*.png;*.jpg;*.bmp;)|*.jpeg;*.png;*.jpg;*.bmp;";
                    else if (currentCBContentTypeSelection == RippleDictionary.ContentType.Video.ToString())
                        dlgBox.Filter = "Videos(*.mp4;*.wmv;)|*.mp4;*.wmv";
                    else if (currentCBContentTypeSelection == RippleDictionary.ContentType.PPT.ToString())
                        dlgBox.Filter = "Presentation Files(*.ppt;*.pptx;)|*.ppt;*.pptx;";
                    else if (currentCBContentTypeSelection == RippleDictionary.ContentType.Text.ToString())
                        MessageBox.Show("Directly enter the text in the Content field");
                    else if (currentCBContentTypeSelection == RippleDictionary.ContentType.HTML.ToString())
                        dlgBox.Filter = "HTML Files(*.htm;*.html;)|*.htm;*.html;";

                    var res = dlgBox.ShowDialog();

                    if (res == System.Windows.Forms.DialogResult.OK)
                    {
                        String targetFolder = Utilities.HelperMethods.TargetAssetsDirectory;
                        String fileExt = System.IO.Path.GetExtension(dlgBox.FileName).ToLower();
                        if (fileExt.Equals(".mp4") || fileExt.Equals(".wmv"))
                            targetFolder += "\\Videos";
                        else if (fileExt.Equals(".jpeg") || fileExt.Equals(".png") || fileExt.Equals(".jpg") || fileExt.Equals(".bmp"))
                            targetFolder += "\\Images";
                        else if (fileExt.Equals(".html") || fileExt.Equals(".htm"))
                        {
                            localHTMLFiles = true;
                            targetFolder += "\\Animations";
                        }
                        else
                            targetFolder += "\\Docs";

                        //Get the complete fileName
                        String updatedFileName = dlgBox.FileName;
                        if (localHTMLFiles)
                        {
                            targetFolder = Utilities.HelperMethods.CopyFolder(System.IO.Path.GetDirectoryName(updatedFileName), targetFolder);
                            updatedFileName = targetFolder + "\\" + System.IO.Path.GetFileName(updatedFileName);
                        }
                        else
                            updatedFileName = Utilities.HelperMethods.CopyFile(dlgBox.FileName, targetFolder);

                        if (!String.IsNullOrEmpty(updatedFileName))
                        {
                            this.ContentValue.Text = updatedFileName;
                            this.ContentValue.ToolTip = updatedFileName;
                        }
                        else
                        {
                            this.ContentValue.Text = "";
                            this.ContentValue.ToolTip = "";
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Please select ContentType as some other value");
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ContentBrowseButton_Click for Screen Properties: {0}", ex.Message);
            }
        }

        private void CBContentTypeValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cb = sender as ComboBox;
                if (cb.SelectedValue != null)
                {
                    //Check visibility of loop video
                    if (cb.SelectedValue.ToString().Equals(RippleDictionary.ContentType.Video.ToString()))
                    {
                        this.LoopVideoValue.Visibility = System.Windows.Visibility.Visible;
                        this.LoopVideoLabel.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this.LoopVideoValue.Visibility = System.Windows.Visibility.Collapsed;
                        this.LoopVideoLabel.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    //Enable/Disable content browse button
                    if (cb.SelectedValue.ToString().Equals(RippleDictionary.ContentType.Text.ToString()) || cb.SelectedValue.ToString().Equals(RippleDictionary.ContentType.Nothing.ToString()))
                    {
                        this.ContentBrowseButton.IsEnabled = false;
                    }
                    else
                        this.ContentBrowseButton.IsEnabled = true;

                    //Enable/Disable the content textbox
                    if (cb.SelectedValue.ToString().Equals(RippleDictionary.ContentType.HTML.ToString()) || cb.SelectedValue.ToString().Equals(RippleDictionary.ContentType.Text.ToString()))
                    {
                        this.ContentValue.IsReadOnlyCaretVisible = false;
                        this.ContentValue.IsReadOnly = false;
                    }
                    else
                    {
                        this.ContentValue.IsReadOnlyCaretVisible = true;
                        this.ContentValue.IsReadOnly = true;
                    }

                    if(cb.SelectedValue.ToString().Equals(RippleDictionary.ContentType.Nothing.ToString()))
                    {
                        //Clear the header
                        this.HeaderValue.Text = "";
                    }

                    //Clear the content box
                    this.ContentValue.Text = "";
                    this.ContentValue.ToolTip = "";
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in CBContentTypeValue_SelectionChanged for Screen Properties: {0}", ex.Message);
            }
        }

        private void ContentValue_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateContentValue();
        }

        private void HeaderValue_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateHeaderValue();
        } 
        #endregion        
    }
}
