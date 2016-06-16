using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<FileInfo> _listaPlikow;
        private List<string> DostepneFormaty;
    
        /// metoda, która jest odpowiedzialna za obsługę formatów graficznych
        public MainWindow()
        {
            InitializeComponent();
            ListaPlikow = new List<FileInfo>();
            DostepneFormaty=new List<String>();
            DostepneFormaty.Add(".JPG");
            DostepneFormaty.Add(".PNG");
            DostepneFormaty.Add(".BMP");
        }
        /// <summary>
        /// Tu następuje ustawienie wartości zmiennej _listaPlikow
        /// </summary>
        public List<FileInfo> ListaPlikow
        {
            get { return _listaPlikow; }
            set {_listaPlikow = value; }
        }
        /// <summary>
        /// Funkcja wywołująca okno dialogowe w którym przegląda się dyski w poszukiwaniu folderów ze zdjęciami
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result.ToString().ToUpper()== "OK")
            {
                ListaPlikow.Clear();
                TreeView.Items.Clear();
                DodajDoTreeList(dialog.SelectedPath);
            }
        }
        /// <summary>
        /// Funkcja sprawdzająca czy rozszerzenie pliku jest zgodne z obsługiwanymi formatami graficznymi
        /// </summary>
        /// <param name="podmiot"></param>
        /// <returns></returns>
        private bool SprawdzRozszerzenie(FileInfo podmiot)
        {
            foreach (var VARIABLE in DostepneFormaty)
            {

                if (podmiot.Extension.ToUpper() == VARIABLE)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Tu również następuje sprawdzanie rozszerzenie i jeśli jest obsługiwane to elementy zostają dodane do listy plików i drzewa plików
        /// </summary>
        /// <param name="folderName"></param>
        private void DodajDoTreeList(string folderName)
        {
            DirectoryInfo Folder;
            FileInfo[] Images;
            TreeView.MouseDoubleClick += TreeViewItem2Click;
            Folder = new DirectoryInfo(folderName);
            Images = Folder.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo VARIABLE in Images)
            {
                if (SprawdzRozszerzenie(VARIABLE))
                {
                    ListaPlikow.Add(VARIABLE);
                    TreeView.Items.Add(VARIABLE);
                }
             
          }
          
        }
        /// <summary>
        /// Widok obiektów drzewa plików
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void TreeViewItem2Click(object sender, RoutedEventArgs eventArgs)
        {
            FileInfo item = (FileInfo)((System.Windows.Controls.TreeView) sender).SelectedItem;
            Uri u = new Uri(item.FullName);
            var b = new BitmapImage();
            b.BeginInit();
            b.CacheOption = BitmapCacheOption.OnLoad;
            b.UriSource = u;
            b.EndInit();
            Image.ToolTip = item.FullName;
            Image.Source = b;
        }
        /// <summary>
        /// Funkcja odpowiedzialna z a podmianę obrazu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageChange(object sender, MouseButtonEventArgs e)
        {
            if (ListaPlikow.Count > 1)
            {
                Uri u = null;
                u = new Uri(ListaPlikow[UzyskajKolejnyjIndexPliku()].FullName);
                var b = new BitmapImage();
                b.BeginInit();
                b.CacheOption = BitmapCacheOption.OnLoad;
                b.UriSource = u;
                b.EndInit();
                Image.Source = b;
                Image.ToolTip = ListaPlikow[UzyskajKolejnyjIndexPliku()].FullName;
            }

        }
        /// <summary>
        /// Uzyskiwanie indeksu pliku
        /// </summary>
        /// <returns></returns>
        private int UzyskajIndexPliku()
        {
            int i = 0;
            foreach (var VARIABLE in ListaPlikow)
            {
                if (VARIABLE.FullName == Image.ToolTip.ToString())
                {
                    i = ListaPlikow.IndexOf(VARIABLE);
                    break;
                }
            }
            return i;
        }
        /// <summary>
        /// Tutaj uzyskiwany jest kolejny indeks pliku
        /// </summary>
        /// <returns></returns>
        private int UzyskajKolejnyjIndexPliku()
        {
            int i = UzyskajIndexPliku();
            i = (i + 1) % (ListaPlikow.Count - 1);
            return i;
        }
        /// <summary>
        /// Usuwanie plików z widoku drzewa i podglądu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsunPlik_Click(object sender, RoutedEventArgs e)
        {
          
            int i = UzyskajIndexPliku();
            string filePath = ListaPlikow[i].FullName;
          if (ListaPlikow.Count == 1)
            {
                Image = null;
            }
            else
            {
                ImageChange(null, null);
            }
            TreeView.Items.RemoveAt(i);
            ListaPlikow.RemoveAt(i);
            File.Delete(filePath);
          
        }
        /// <summary>
        /// Funkcja odpowiedzialna za obracanie obrazu o zadaną wartość w stopniach (90 lewo,90 prawo i180)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Obroc (object sender, RoutedEventArgs e)
        {
            var item = (System.Windows.Controls.MenuItem) sender;
            var wariant = item.Header.ToString();
            int i = UzyskajIndexPliku();
            string filePath = ListaPlikow[i].FullName;
            var u = new Uri(filePath);
            var old = (BitmapImage)Image.Source;
            var b = new BitmapImage();
            b.BeginInit();
            b.Rotation = old.Rotation;
            b.CacheOption = BitmapCacheOption.OnLoad;
            b.UriSource = u;
            if (wariant == "Obróć o 90 w lewo")
            {
                if (b.Rotation == Rotation.Rotate0) b.Rotation = Rotation.Rotate270;
                else
                if (b.Rotation == Rotation.Rotate90) b.Rotation = Rotation.Rotate0;
                else
                if (b.Rotation == Rotation.Rotate180) b.Rotation = Rotation.Rotate90;
                else
                if (b.Rotation == Rotation.Rotate270) b.Rotation = Rotation.Rotate180;
            }
            if (wariant == "Obróć o 90 w prawo")
            {
                if (b.Rotation == Rotation.Rotate0) b.Rotation = Rotation.Rotate90;
                else
                if (b.Rotation == Rotation.Rotate90) b.Rotation = Rotation.Rotate180;
                else
                if (b.Rotation == Rotation.Rotate180) b.Rotation = Rotation.Rotate270;
                else
                if (b.Rotation == Rotation.Rotate270) b.Rotation = Rotation.Rotate0;
            }
            if (wariant == "Obróć o 180")
            {
                if (b.Rotation == Rotation.Rotate0) b.Rotation = Rotation.Rotate180;
                else
                if (b.Rotation == Rotation.Rotate90) b.Rotation = Rotation.Rotate270;
                else
                if (b.Rotation == Rotation.Rotate180) b.Rotation = Rotation.Rotate0;
                else
                if (b.Rotation == Rotation.Rotate270) b.Rotation = Rotation.Rotate90;
            }
            b.EndInit();
            Image.Source = b;
        }
        /// <summary>
        /// Wyświetlenie informacji o programie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Przeglądarka grafiki ver. 1.0.0 || Autor: Sylwester Nowosiad (53928)", "O programie...",MessageBoxButton.OK, MessageBoxImage.Information);
        }

       private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Wyświetlenie pliku pomocy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
           Window1 help = new Window1();
           help.Show();
        }
      
    }
}
