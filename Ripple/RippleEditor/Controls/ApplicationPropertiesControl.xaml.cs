using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ApplicationPropertiesControl.xaml
    /// </summary>
    public partial class ApplicationPropertiesControl : UserControl
    {
        private static String prevSelectedUnlockMode = String.Empty;

        public ApplicationPropertiesControl()
        {
            InitializeComponent();
            InitializeControls();
        }

        #region Common Control Methods
        public void InitializeControls()
        {
            try
            {
                //Initialize floor animation types
                this.CBAnimationTypeValue.Items.Clear();
                foreach (var animType in Enum.GetNames(typeof(RippleDictionary.AnimationType)))
                {
                    this.CBAnimationTypeValue.Items.Add(animType);
                }
                this.CBAnimationTypeValue.SelectedValue = RippleDictionary.AnimationType.Flash.ToString();

                //Initialize unlock modes
                this.CBUnlockModeValue.Items.Clear();
                foreach (var unMode in Enum.GetNames(typeof(RippleDictionary.Mode)))
                {
                    this.CBUnlockModeValue.Items.Add(unMode);
                }
                prevSelectedUnlockMode = RippleDictionary.Mode.Gesture.ToString();
                this.CBUnlockModeValue.SelectedValue = RippleDictionary.Mode.Gesture.ToString();

                //Initialize gesture unlock types for gesture as the selected unlock mode
                this.CBUnlockTypeValue.Items.Clear();
                foreach (var unType in Enum.GetNames(typeof(RippleDictionary.GestureUnlockType)))
                {
                    this.CBUnlockTypeValue.Items.Add(unType);
                }
                this.CBUnlockTypeValue.SelectedValue = RippleDictionary.GestureUnlockType.LeftSwipe.ToString();

                this.TBAnimationContentValue.Text = "";
                this.TBLockScreenContentValue.Text = "";
                this.TBAnimationContentValue.ToolTip = "";
                this.TBLockScreenContentValue.ToolTip = "";
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in InitializeControls: {0}", ex.Message);
            }
        }

        public bool ValidateControl()
        {
            try
            {
                //Animation Content cannot be empty
                String animationContent = TBAnimationContentValue.Text;
                if (String.IsNullOrEmpty(animationContent))
                {
                    MessageBox.Show("Please enter valid value for Animation URI, as it is required in case of Animation Type = Flash / HTML");
                    return false;
                }

                //Validate the value for Animation content
                //Web URI
                if (this.CBAnimationTypeValue.SelectedValue.ToString().Equals("HTML") && animationContent.StartsWith("http"))
                {
                    //Do nothing - Valid
                    /*([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?*/
                }
                else
                {
                    //Local HTML URI
                    if (this.CBAnimationTypeValue.SelectedValue.ToString().Equals(RippleDictionary.AnimationType.HTML.ToString()) && !(animationContent.EndsWith(".htm") || animationContent.EndsWith(".html")))
                    {
                        MessageBox.Show("Please enter valid value for HTML based URI, it should have an extension html or htm for local files, for web hosted files it should start with http");
                        return false;
                    }
                    //Local swf file
                    else if(this.CBAnimationTypeValue.SelectedValue.ToString().Equals(RippleDictionary.AnimationType.Flash.ToString()) && (!animationContent.EndsWith(".swf")))
                    {
                        MessageBox.Show("Please enter valid value for Flash based URI, it should have an extension swf only");
                        return false;
                    }
                }

                //Lock Screen Content cannot be empty
                if (String.IsNullOrEmpty(this.TBLockScreenContentValue.Text))
                {
                    MessageBox.Show("Please enter valid value for Lock Screen Content URI, as it is required");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in ValidateControl: {0}", ex.Message);
                return false;
            }
        }

        public void SaveApplicationProperties()
        {
            try
            {
                //Animation type
                MainPage.rippleData.Floor.Start.Animation.AnimType = (RippleDictionary.AnimationType)this.CBAnimationTypeValue.SelectedIndex;

                //Unlock Mode
                MainPage.rippleData.Floor.Start.Unlock.Mode = (RippleDictionary.Mode)this.CBUnlockModeValue.SelectedIndex;

                //Animation content
                MainPage.rippleData.Floor.Start.Animation.Content = this.TBAnimationContentValue.Text;

                //Lock screen content
                MainPage.rippleData.Screen.ScreenContents["LockScreen"].Content = this.TBLockScreenContentValue.Text;

                try
                {
                    //Unlock type
                    MainPage.rippleData.Floor.Start.Unlock.UnlockType = this.CBUnlockTypeValue.SelectedValue.ToString();
                }
                catch(Exception)
                {}
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in SaveApplicationProperties: {0}", ex.Message);
            }           
        }

        public void SetApplicationProperties()
        {
            try
            {
                //Set the Animation Type
                this.CBAnimationTypeValue.SelectedValue = MainPage.rippleData.Floor.Start.Animation.AnimType.ToString();

                //Set the Unlock Mode
                this.CBUnlockModeValue.SelectedValue = MainPage.rippleData.Floor.Start.Unlock.Mode.ToString();
                this.CBUnlockModeValue.SelectedValue = MainPage.rippleData.Floor.Start.Unlock.Mode.ToString();

                //Set the Unlock Type
                this.CBUnlockTypeValue.SelectedValue = MainPage.rippleData.Floor.Start.Unlock.UnlockType.ToString();

                //Set the Animation content
                if (!MainPage.rippleData.Floor.Start.Animation.Content.StartsWith(@"\Assets\"))
                    this.TBAnimationContentValue.Text = MainPage.rippleData.Floor.Start.Animation.Content;
                else
                    this.TBAnimationContentValue.Text = Utilities.HelperMethods.TargetAssetsRoot + MainPage.rippleData.Floor.Start.Animation.Content;

                //Set the Lock Screen content
                if (!MainPage.rippleData.Screen.ScreenContents["LockScreen"].Content.StartsWith(@"\Assets\"))
                    this.TBLockScreenContentValue.Text = MainPage.rippleData.Screen.ScreenContents["LockScreen"].Content;
                else
                    this.TBLockScreenContentValue.Text = Utilities.HelperMethods.TargetAssetsRoot + MainPage.rippleData.Screen.ScreenContents["LockScreen"].Content;
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in SetApplicationProperties: {0}", ex.Message);
            }
        } 
        #endregion

        #region UI Methods
        private void AnimationContentBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.CBAnimationTypeValue.SelectedValue == RippleDictionary.AnimationType.Flash.ToString())
                {
                    //Opportunity to browse for Animation files
                    System.Windows.Forms.OpenFileDialog dlgBox = new System.Windows.Forms.OpenFileDialog();
                    dlgBox.Filter = "Flash files(*.swf;)|*.swf;";
                    var res = dlgBox.ShowDialog();
                    if (res == System.Windows.Forms.DialogResult.OK)
                    {
                        String updatedFileName = Utilities.HelperMethods.CopyFile(dlgBox.FileName, Utilities.HelperMethods.TargetAssetsDirectory + "\\Animations");

                        if (!String.IsNullOrEmpty(updatedFileName))
                        {
                            this.TBAnimationContentValue.Text = updatedFileName;
                            this.TBAnimationContentValue.ToolTip = updatedFileName;
                        }
                        else
                        {
                            this.TBAnimationContentValue.Text = "";
                            this.TBAnimationContentValue.ToolTip = "";
                        }
                    }
                }
                else if (this.CBAnimationTypeValue.SelectedValue == RippleDictionary.AnimationType.HTML.ToString())
                {
                    //Opportunity to browse for Animation files
                    System.Windows.Forms.OpenFileDialog dlgBox = new System.Windows.Forms.OpenFileDialog();
                    dlgBox.Filter = "HTML files(*.html;*.htm;)|*.html;*.htm;";
                    var res = dlgBox.ShowDialog();
                    if (res == System.Windows.Forms.DialogResult.OK)
                    {
                        String updatedFileName = dlgBox.FileName;
                        String targetfolder = Utilities.HelperMethods.CopyFolder(System.IO.Path.GetDirectoryName(updatedFileName), Utilities.HelperMethods.TargetAssetsDirectory + "\\Animations");
                        updatedFileName = targetfolder + "\\" + System.IO.Path.GetFileName(updatedFileName);
                        if (!String.IsNullOrEmpty(updatedFileName))
                        {
                            this.TBAnimationContentValue.Text = updatedFileName;
                            this.TBAnimationContentValue.ToolTip = updatedFileName;
                        }
                        else
                        {
                            this.TBAnimationContentValue.Text = "";
                            this.TBAnimationContentValue.ToolTip = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in AnimationContentBrowseButton_Click: {0}", ex.Message);
            }
        }

        //TODO: Lock screen content could support other content types as well.
        private void LockScreenContentBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Opportunity to browse for content files
                System.Windows.Forms.OpenFileDialog dlgBox = new System.Windows.Forms.OpenFileDialog();
                dlgBox.Filter = "Media Files(*.mp4;*.wmv;*.jpeg;*.png;*.jpg;*.bmp;)|*.mp4;*.wmv;*.jpeg;*.png;*.jpg;*.bmp;|Videos(*.mp4;*.wmv;)|*.mp4;*.wmv;|Images(*.jpeg;*.png;*.jpg;*.bmp;)|*.jpeg;*.png;*.jpg;*.bmp;";

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
                        this.TBLockScreenContentValue.Text = updatedFileName;
                        this.TBLockScreenContentValue.ToolTip = updatedFileName;
                    }
                    else
                    {
                        this.TBLockScreenContentValue.Text = "";
                        this.TBLockScreenContentValue.ToolTip = "";
                    }
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in LockScreenContentBrowseButton_Click: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Selection Changed event for the Unlock modes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CBUnlockModeValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cb = sender as ComboBox;
                if (cb.SelectedValue == null)
                    return;

                //Gesture selected
                if (cb.SelectedValue.ToString() == RippleDictionary.Mode.Gesture.ToString())
                {
                    string g = this.CBUnlockModeValue.SelectedValue.ToString();
                    //Get the previously selected unlock mode
                    prevSelectedUnlockMode = this.CBUnlockModeValue.SelectedValue.ToString();

                    //Set the unlock types as gesture types - select Right Swipe as default
                    this.CBUnlockTypeValue.Items.Clear();
                    foreach (var unType in Enum.GetNames(typeof(RippleDictionary.GestureUnlockType)))
                    {
                        this.CBUnlockTypeValue.Items.Add(unType);
                    }
                    this.CBUnlockTypeValue.SelectedValue = RippleDictionary.GestureUnlockType.RightSwipe.ToString();
                }
                //Animation Selected
                else if (cb.SelectedValue.ToString() == RippleDictionary.Mode.HTML.ToString())
                {
                    //Get the previously selected unlock mode
                    prevSelectedUnlockMode = this.CBUnlockModeValue.SelectedValue.ToString();

                    //Set the unlock types as HTMl invoke
                    this.CBUnlockTypeValue.Items.Clear();
                    this.CBUnlockTypeValue.Items.Add("HTMLInvoke");
                    this.CBUnlockTypeValue.SelectedValue = "HTMLInvoke";
                }
                else
                {
                    //Set the selection to previous selected
                    cb.SelectedValue = prevSelectedUnlockMode;
                }
            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in CBUnlockModeValue_SelectionChanged: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Selection Changed event for type of Unlock inside the modes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CBUnlockTypeValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Do nothing right now
        }

        /// <summary>
        /// Selection changed event for the main Lock Screen Animation Type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CBAnimationTypeValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cb = sender as ComboBox;
                if (cb.SelectedValue == null)
                    return;

                if (cb.SelectedValue.ToString() == RippleDictionary.AnimationType.Flash.ToString() || cb.SelectedValue.ToString() == RippleDictionary.AnimationType.HTML.ToString())
                {
                    //Set unlock mode as gesture
                    this.CBUnlockModeValue.SelectedValue = RippleDictionary.Mode.Gesture.ToString();

                    //Set Unlock type as gesture types - default right swipe
                    this.CBUnlockTypeValue.Items.Clear();
                    foreach (var unType in Enum.GetNames(typeof(RippleDictionary.GestureUnlockType)))
                    {
                        this.CBUnlockTypeValue.Items.Add(unType);
                    }
                    this.CBUnlockTypeValue.SelectedValue = RippleDictionary.GestureUnlockType.RightSwipe.ToString();

                    //Clear the animation content selected
                    this.TBAnimationContentValue.Text = "";
                    this.TBAnimationContentValue.ToolTip = "";
                }
                else if (cb.SelectedValue.ToString() == RippleDictionary.AnimationType.HTML.ToString())
                {
                    //Set unlock mode as Animation - default
                    this.CBUnlockModeValue.SelectedValue = RippleDictionary.Mode.HTML.ToString();

                    //Set Unlock type as fixed HTML invoke value
                    this.CBUnlockTypeValue.Items.Clear();
                    this.CBUnlockTypeValue.Items.Add("HTMLInvoke");

                    this.CBUnlockTypeValue.SelectedValue = "HTMLInvoke";

                    //Clear the animation content selected
                    this.TBAnimationContentValue.Text = "";
                    this.TBAnimationContentValue.ToolTip = "";
                }

            }
            catch (Exception ex)
            {
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in CBAnimationTypeValue_SelectionChanged: {0}", ex.Message);
            }            
        } 
        #endregion

    }
}
