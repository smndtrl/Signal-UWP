using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Signal.Models;

namespace Signal.Xaml.Controls
{
    public sealed partial class AvatarView : UserControl
    {
        public static readonly DependencyProperty RecipientProperty = DependencyProperty.Register("Type", typeof(Recipient), typeof(AvatarView), new PropertyMetadata(new Recipient(), new PropertyChangedCallback(OnRecipientChanged)));
        public Recipient Recipient
        {
            get { return (Recipient)GetValue(RecipientProperty); }
            set { SetValue(RecipientProperty, value); }
        }

        private static void OnRecipientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AvatarView;

            control?.Update((Recipient)e.NewValue);
        }
        public AvatarView()
        {
            this.InitializeComponent();

            VisualStateManager.GoToState(this, "DefaultState", false);
        }

        private void Update(Recipient recipient)
        {

        }
    }
}
