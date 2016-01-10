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

    public MainWindow()
    {
      InitializeComponent();

      addKeyButtons(keyCharactersZawgyi, 16);
    }

    private void addKeyButtons(string keyCharacters, int keysPerRow)
    {
      int numRows = (keyCharacters.Length + keysPerRow - 1) / keysPerRow;

      keyboardGrid.Children.Clear();
      keyboardGrid.RowDefinitions.Clear();
      keyboardGrid.ColumnDefinitions.Clear();

      for (int i = 0; i < numRows; i++)
      {
        keyboardGrid.RowDefinitions.Add(new RowDefinition());
      }

      for (int i = 0; i < keysPerRow; i++)
      {
        keyboardGrid.ColumnDefinitions.Add(new ColumnDefinition());
      }

      for (int i = 0; i < keyCharacters.Length; i++)
      {
        char character = keyCharacters[i];

        Button button = new Button()
        {
          Content = character,
          Background = new SolidColorBrush(Colors.Black),
          Foreground = new SolidColorBrush(Colors.White),
          BorderThickness = new Thickness(),
        };

        Grid.SetRow(button, i / keysPerRow);
        Grid.SetColumn(button, i % keysPerRow);

        button.Click += (object sender, RoutedEventArgs e) =>
        {
          typeCharacter(character);
        };

        keyboardGrid.Children.Add(button);
      }
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
