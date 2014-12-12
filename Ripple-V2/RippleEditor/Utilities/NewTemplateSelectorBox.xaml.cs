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
using System.Windows.Shapes;

namespace RippleEditor.Utilities
{
    /// <summary>
    /// Interaction logic for NewTemplateSelectorBox.xaml
    /// </summary>
    public partial class NewTemplateSelectorBox : Window
    {
        public TemplateOptions SelectedItem { get; set; }
        public NewTemplateSelectorBox()
        {
            InitializeComponent();
            this.TemplateOptionsBox.Items.Clear();
            foreach (var tempType in Enum.GetNames(typeof(TemplateOptions)))
            {
                this.TemplateOptionsBox.Items.Add(tempType);
            }
            TemplateOptionsBox.SelectedIndex = 0;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedItem = (TemplateOptions)this.TemplateOptionsBox.SelectedIndex;
            this.DialogResult = true;
        }

        #region Header Drag Bar
        private void CloseButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
