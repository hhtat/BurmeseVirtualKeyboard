using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace BurmeseVirtualKeyboard
{
    public partial class MainWindow : Window
    {
        private const string keyCharactersZawgyi = "ကခဂဃငစဆဇဈဉညဋဌဍဎဏတထဒဓနပဖဗဘမယရ႐လဝသႆဟဠအ၏ဤဥဦဧဩဪ၌၍၎႑႒ဣါၚာိီုူေဲဳဴွံ့း္်ၼြၱၶၻ၀၁၂၃၄၅၆၇၈၉၊။ၽၾၿႀႁႂႃႄျ႔႕႖႗ၤၦၧၱၲၷ႖ၼဤ၌ၸၠဉ၍ၪႆၥၰဈၺၽႇႎႌႃႄႉႍႋၵၶၹၨၳၴၡၣႅၻၫၩႁႂ";
        private const int numKeysPerRow = 24;

        private readonly FontFamily zawgyiOne = new FontFamily(new Uri("pack://application:,,,/"), "resources/#Zawgyi-One");

        private bool openedState = false;

        public MainWindow()
        {
            InitializeComponent();

            addKeyButtons();
            toggleState();
        }

        private void toggleState()
        {
            openedState = !openedState;

            int numRows = (keyCharactersZawgyi.Length + numKeysPerRow - 1) / numKeysPerRow;

            if (openedState)
            {
                closedGrid.Visibility = Visibility.Collapsed;

                Top = 0;

                Height = numRows * SystemParameters.PrimaryScreenWidth / numKeysPerRow;
                Width = SystemParameters.PrimaryScreenWidth;

                openedGrid.Visibility = Visibility.Visible;
            }
            else
            {
                openedGrid.Visibility = Visibility.Collapsed;

                Top = (numRows - 1) * SystemParameters.PrimaryScreenWidth / numKeysPerRow;

                Height = SystemParameters.PrimaryScreenWidth / numKeysPerRow;
                Width = SystemParameters.PrimaryScreenWidth / numKeysPerRow;

                closedGrid.Visibility = Visibility.Visible;
            }

            Left = SystemParameters.PrimaryScreenWidth - Width;
        }

        private void addKeyButtons()
        {
            int numRows = (keyCharactersZawgyi.Length + numKeysPerRow - 1) / numKeysPerRow;

            openedGrid.Children.Clear();
            openedGrid.RowDefinitions.Clear();
            openedGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < numRows; i++)
            {
                openedGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < numKeysPerRow; i++)
            {
                openedGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < keyCharactersZawgyi.Length; i++)
            {
                char character = keyCharactersZawgyi[i];

                addButton(
                    new TextBlock()
                    {
                        Text = character.ToString(),
                        TextAlignment = TextAlignment.Center,
                        FontSize = 42.0,
                        FontFamily = zawgyiOne,
                        Width = 100.0,
                        Height = 100.0,
                        Padding = new Thickness(0.0, 14.0, 0.0, 0.0),
                    },
                    openedGrid,
                    i / numKeysPerRow,
                    i % numKeysPerRow,
                    (object sender, RoutedEventArgs e) =>
                    {
                        typeCharacter(character);
                    });
            }

            addButton(
                new TextBlock()
                {
                    Text = "✖",
                    TextAlignment = TextAlignment.Center,
                    FontSize = 42.0,
                    Width = 100.0,
                    Height = 100.0,
                    Padding = new Thickness(0.0, 20.0, 0.0, 0.0),
                },
                openedGrid,
                numRows - 1,
                numKeysPerRow - 2,
                (object sender, RoutedEventArgs e) =>
                {
                    Close();
                });

            addButton(
                new TextBlock()
                {
                    Text = "❱",
                    TextAlignment = TextAlignment.Center,
                    FontSize = 42.0,
                    Width = 100.0,
                    Height = 100.0,
                    Padding = new Thickness(0.0, 20.0, 0.0, 0.0),
                },
                openedGrid,
                numRows - 1,
                numKeysPerRow - 1,
                (object sender, RoutedEventArgs e) =>
                {
                    toggleState();
                });

            addButton(
                new TextBlock()
                {
                    Text = "❰",
                    TextAlignment = TextAlignment.Center,
                    FontSize = 42.0,
                    Width = 100.0,
                    Height = 100.0,
                    Padding = new Thickness(0.0, 20.0, 0.0, 0.0),
                },
                closedGrid,
                1,
                1,
                (object sender, RoutedEventArgs e) =>
                {
                    toggleState();
                });
        }

        private void addButton(UIElement content, Grid grid, int row, int column, RoutedEventHandler clickHandler)
        {
            Button button = new Button()
            {
                Content = content,
            };

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);

            button.Click += clickHandler;

            grid.Children.Add(button);
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

        private static void typeCharacter(char character)
        {
            NativeMethods.KeyboardInput keyboardInputDown = new NativeMethods.KeyboardInput
            {
                wVk = 0,
                wScan = character,
                dwFlags = NativeMethods.KeyboardInputFlags.KEYEVENTF_UNICODE,
                time = 0,
                dwExtraInfo = UIntPtr.Zero,
            };

            sendKeboyardInput(keyboardInputDown);

            NativeMethods.KeyboardInput keyboardInputUp = new NativeMethods.KeyboardInput
            {
                wVk = 0,
                wScan = character,
                dwFlags = NativeMethods.KeyboardInputFlags.KEYEVENTF_UNICODE | NativeMethods.KeyboardInputFlags.KEYEVENTF_KEYUP,
                time = 0,
                dwExtraInfo = UIntPtr.Zero,
            };

            sendKeboyardInput(keyboardInputUp);
        }

        private static void sendKeboyardInput(NativeMethods.KeyboardInput keyboardInput)
        {
            NativeMethods.Input input = new NativeMethods.Input
            {
                type = NativeMethods.InputType.INPUT_KEYBOARD,
                ki = keyboardInput,
            };

            NativeMethods.SendInput(
                1,
                new NativeMethods.Input[] { input },
                Marshal.SizeOf(typeof(NativeMethods.Input)));
        }
    }
}
