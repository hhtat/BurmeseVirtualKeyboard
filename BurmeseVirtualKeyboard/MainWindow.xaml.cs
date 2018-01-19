using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using WindowsInput;

namespace BurmeseVirtualKeyboard
{
    public partial class MainWindow : Window
    {
        private const string keyCharactersZawgyi = "ကခဂဃငစဆဇဈဉညဋဌဍဎဏတထဒဓနပဖဗဘမယရ႐လဝသႆဟဠအ၏ဤဥဦဧဩဪ၌၍၎႑႒ဣါၚာိီုူေဲဳဴွံ့း္်ၼြၱၶၻ၀၁၂၃၄၅၆၇၈၉၊။ၽၾၿႀႁႂႃႄျ႔႕႖႗ၤၦၧၱၲၷ႖ၼဤ၌ၸၠဉ၍ၪႆၥၰဈၺၽႇႎႌႃႄႉႍႋၵၶၹၨၳၴၡၣႅၻၫၩႁႂ";
        private const int numKeysPerRow = 24;
        private const int numRowsClosed = 3;

        private static readonly FontFamily zawgyiOne = new FontFamily(new Uri("pack://application:,,,/"), "resources/#Zawgyi-One");
        private static readonly int numRows = (keyCharactersZawgyi.Length + numKeysPerRow - 1) / numKeysPerRow;

        private readonly InputSimulator input = new InputSimulator();

        private bool openedState = false;
        private double windowY = 0.0;

        private bool customDragMove;
        private Point customDragMoveOrigin;

        public MainWindow()
        {
            InitializeComponent();

            addKeyButtons();
            toggleState();
        }

        private void toggleState()
        {
            openedState = !openedState;

            if (openedState)
            {
                closedGrid.Visibility = Visibility.Collapsed;

                Height = numRows * SystemParameters.PrimaryScreenWidth / numKeysPerRow;
                Width = SystemParameters.PrimaryScreenWidth;

                openedGrid.Visibility = Visibility.Visible;
            }
            else
            {
                openedGrid.Visibility = Visibility.Collapsed;

                Height = numRowsClosed * SystemParameters.PrimaryScreenWidth / numKeysPerRow;
                Width = SystemParameters.PrimaryScreenWidth / numKeysPerRow;

                closedGrid.Visibility = Visibility.Visible;
            }

            updateTop();
            Left = SystemParameters.PrimaryScreenWidth - Width;
        }

        private void showInfo()
        {
            AboutDialog dialog = new AboutDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void updateTop()
        {
            if (openedState)
            {
                Top = windowY;
            }
            else
            {
                Top = windowY + (numRows - 1) * SystemParameters.PrimaryScreenWidth / numKeysPerRow;
            }
        }

        private void addKeyButtons()
        {
            int numRows = (keyCharactersZawgyi.Length + numKeysPerRow - 1) / numKeysPerRow;

            for (int i = 0; i < numRows; i++)
            {
                openedGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < numKeysPerRow; i++)
            {
                openedGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < numRowsClosed; i++)
            {
                closedGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < keyCharactersZawgyi.Length; i++)
            {
                char character = keyCharactersZawgyi[i];

                addButton(
                    createButtonText(character.ToString(), zawgyiOne, 14.0),
                    openedGrid,
                    i / numKeysPerRow,
                    i % numKeysPerRow,
                    (sender, e) => input.Keyboard.TextEntry(character));
            }

            setupMoveControl(addButton(createButtonText("⭥"), openedGrid, numRows - 1, numKeysPerRow - 2));
            addButton(createButtonText("❯"), openedGrid, numRows - 1, numKeysPerRow - 1, (sender, e) => toggleState());

            addButton(createButtonText("❮"), closedGrid, 0, 0, (sender, e) => toggleState());
            addButton(createButtonText("ℹ"), closedGrid, 1, 0, (sender, e) => showInfo());
            addButton(createButtonText("✕"), closedGrid, 2, 0, (sender, e) => Close());
        }

        private void setupMoveControl(UIElement control)
        {
            control.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (control.CaptureMouse())
                {
                    customDragMove = true;
                    customDragMoveOrigin = e.GetPosition(this);
                }
            };

            control.PreviewMouseMove += (sender, e) =>
            {
                if (!customDragMove) return;

                double deltaY = e.GetPosition(this).Y - customDragMoveOrigin.Y;
                windowY += deltaY;
                updateTop();
            };

            control.PreviewMouseLeftButtonUp += (sender, e) =>
            {
                if (customDragMove)
                {
                    customDragMove = false;
                    control.ReleaseMouseCapture();
                }
            };
        }

        private Button addButton(UIElement content, Grid grid, int row, int column, RoutedEventHandler clickHandler = null)
        {
            Button button = new Button()
            {
                Content = content,
            };

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);

            if (clickHandler != null)
            {
                button.Click += clickHandler;
            }

            grid.Children.Add(button);

            return button;
        }

        private TextBlock createButtonText(string text, FontFamily fontFamily = null, double topPadding = 20.0)
        {
            TextBlock textBlock = new TextBlock()
            {
                Text = text,
                TextAlignment = TextAlignment.Center,
                FontSize = 42.0,
                Width = 100.0,
                Height = 100.0,
                Padding = new Thickness(0.0, topPadding, 0.0, 0.0),
            };

            if (fontFamily != null)
            {
                textBlock.FontFamily = fontFamily;
            }

            return textBlock;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper interopHelper = new WindowInteropHelper(this);

            NativeMethods.WindowStyle style = NativeMethods.GetWindowLong(
                interopHelper.Handle,
                NativeMethods.WindowLong.GWL_EXSTYLE);

            style |= NativeMethods.WindowStyle.WS_EX_NOACTIVATE;

            NativeMethods.SetWindowLong(interopHelper.Handle,
                NativeMethods.WindowLong.GWL_EXSTYLE,
                style);
        }
    }
}
