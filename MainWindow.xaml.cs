using Microsoft.Win32;
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

namespace A02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Clipboard.Clear();
        }
        // Declaring Variables
        private bool fileNotSaved = true;
        private string fileCurrent = "";
        private bool exitByExitButton = false;

    /// <summary>
    /// It is a event handler for the New menu item.
    /// Clears the text editor, prompts the user if unsaved modifications exist,
    /// and resets the current file and window title to start a new document.
    /// </summary>
    /// <param name="sender">The event sender</param>
    /// <param name="e">The event argument</param>
        private void newFile_Click(object sender, RoutedEventArgs e)
        {
            if (SaveChecker() != 0)
            {
                TextEditor.Clear();
                fileCurrent = "";
                updateTitle("Untitled");
            }
        }

        /// <summary>
        /// It is a event handler for the Open menu item.
        /// Prompts the user to save changes if unsaved modifications exist,
        /// opens a file dialog to select a text file, and loads its content
        /// into the text editor.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event argument</param>
        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            if (SaveChecker() != 0)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    fileCurrent = openFileDialog.FileName;
                    TextEditor.Text = System.IO.File.ReadAllText(openFileDialog.FileName);
                }
            }
        }

        /// <summary>
        /// It is a event handler for the Save menu item.
        /// Saves the current content to an existing file if one exists.
        /// or prompts the user to specify a new file location using the "Save As" dialog
        /// and updates the window title and marks the file as saved.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event argument</param>
        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(fileCurrent))
            {
                saveAsFile_Click(sender, e); // Calling SaveAsFile to prompt the user to save the file.
            }
            else
            {
                System.IO.File.WriteAllText(fileCurrent, TextEditor.Text); // Save content to the current file
                updateTitle(fileCurrent);                                  // update the title with the current file name.
            } 
            fileNotSaved = false;  // File is saved now.
        }

        /// <summary>
        /// It is a event handler for the SaveAs menu item.
        /// Shows the Save File dialog box to allow the user to specify a new file location
        /// and saves the current content to the selected location.
        /// and updates the window title with the new name.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event argument</param>
        private void saveAsFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveAsFileDialog = new SaveFileDialog();
            saveAsFileDialog.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveAsFileDialog.ShowDialog() == true)
            {
                fileCurrent = saveAsFileDialog.FileName;                   // Update the current file path
                System.IO.File.WriteAllText(fileCurrent, TextEditor.Text); // Save content to the selected file.
                updateTitle(fileCurrent);                                  // Update window Title with the new name.
                fileNotSaved = false;                                      // File is saved now
            } 
        }

        /// <summary>
        /// It is a event handler for the Exit menu item.
        /// Sets the 'exitByExitButton' flag to indicate that the exit operation was triggered by the 
        /// application's close button. Checks for unsaved changes, and if none exist or the user chooses 
        /// not to save, closes the application.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event argument</param>
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            exitByExitButton = true;   // if the application is exit by close button
            int status = SaveChecker();
            if(status == 1)
            {
               Close();               // Close the application if nothing to saved.
            }
        }

        /// <summary>
        /// It is a event handler for the window's closing event triggered by clicking the application's close button.
        /// Checks if the application is closing due to the user clicking the close button
        /// and if so, checks for unsaved changes.If unsaved changes exist, it prompts the user to save or cancel 
        /// the exit operation.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event argument</param>
        private void ClosingX(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(exitByExitButton == false)
            {
                int status;
                status = SaveChecker();
                if (status == 0)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Updates the title of the application window to display the name of the current file.
        /// </summary>
        /// <param name="file">The name of the current file or "Untitled" if no file is open.</param>
        private void updateTitle(string file)
        {
            string fileName = System.IO.Path.GetFileName(file);
            this.Title = fileName; // Update the window title with file name.
        }

        /// <summary>
        /// Event handler for the text content changed in the text editor.
        /// Calculates the length of the text in the editor and updates the status bar
        /// to display the current character count.
        /// </summary>
        /// <param name="sender">The event sender (the text editor).</param>
        /// <param name="e">The event arguments.</param>
        private void TextChangedEventHandler(object sender,EventArgs e)
        {
             string text = TextEditor.Text;
             int length = text.Length;
             labelStatus.Content = $"Count: {length}";  // Update the status bar with character count
        }

        /// <summary>
        /// Checks if there are unsaved changes in the text editor and prompts the user for action.
        /// If unsaved changes exist, displays a confirmation dialog to save, discard, or cancel the operation.
        /// </summary>
        /// <returns>An integer representing the user's choice or the absence of unsaved changes.</returns>
        private int SaveChecker()
        {
           if(fileNotSaved == true)
            {
                var save = MessageBox.Show("Do you want to save changes", "Notepad", MessageBoxButton.YesNoCancel);
                if (save == MessageBoxResult.Yes)
                {
                    saveAsFile_Click(this,new RoutedEventArgs()); // Prompt user to save.
                    return 2;                                     // Choice to save changes
                }else if (save == MessageBoxResult.No)
                {
                    return 1;                                     // Choice to discard changes and close
                }else if(save == MessageBoxResult.Cancel)
                {
                    return 0;                                     // Choice to cancel the operation.
                }
            }
            return 3;
        }

        /// <summary>
        /// Event handler for the "About" menu item.
        /// Displays an About Box, typically containing information about the application.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void about_Click(object sender, RoutedEventArgs e)
        {
            about About = new about();
            About.Show();  // Display the About Box as a modal dialog.
        }
    }
}