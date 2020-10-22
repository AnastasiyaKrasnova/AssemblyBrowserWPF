using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AssemblyBrowser;

namespace AssemblyBrowserWPF
{
    class ViewModel : INotifyPropertyChanged
    {
        private RelayCommand openFileCommand;
        public RelayCommand OpenFileCommand
        {
            get
            {
                return openFileCommand ?? (openFileCommand = new RelayCommand(obj =>
                {
                    IDialogService dialogService = new DialogService();
                    if (dialogService.OpenFileDialog())
                    {
                        FilePath = dialogService.FilePath;
                        BrowseAssembly();
                    }
                }));
            }
        }

        private RelayCommand closeWindowCommand;
        public RelayCommand CloseWindowCommand
        {
            get
            {
                return closeWindowCommand ??
                    (closeWindowCommand = new RelayCommand(obj =>
                    {
                        Window wnd = obj as Window;
                        if (wnd != null)
                        {
                            wnd.Close();
                        }
                    }));
            }
        }

        private string assemblyName;
        public string AssemblyName
        {
            get { return assemblyName; }
            set
            {
                assemblyName = value;
                OnPropertyChanged("AssemblyName");
            }
        }

        private IEnumerable<NamespaceInfo> assemblyData;
        public IEnumerable<NamespaceInfo> AssemblyData
        {
            get { return assemblyData; }
            set
            {
                assemblyData = value;
                OnPropertyChanged("AssemblyData");
            }
        }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        private void BrowseAssembly()
        {
            AsmBrowser asmBrowser = new AsmBrowser();
            AssemblyInfo browseResult = asmBrowser.CollectAssemblyInfo(filePath);
            AssemblyName = browseResult.AssemblyName;
            AssemblyData = browseResult.Namespaces;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
