using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmallGreen.Desktop.Settings.BlackControl
{
    public class ChemicalBucket : Button
    {
        static ChemicalBucket()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChemicalBucket), new FrameworkPropertyMetadata(typeof(ChemicalBucket)));
        }

        public static readonly DependencyProperty BucketCodeProperty =
            DependencyProperty.Register(nameof(BucketCode), typeof(string), typeof(ChemicalBucket), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty LevelValueProperty =
            DependencyProperty.Register(nameof(LevelValue), typeof(float), typeof(ChemicalBucket), new PropertyMetadata(0f, OnLevelChanged));

        public static readonly DependencyProperty MaxLevelProperty =
            DependencyProperty.Register(nameof(MaxLevel), typeof(float), typeof(ChemicalBucket), new PropertyMetadata(1000f, OnLevelChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(ChemicalBucket), new PropertyMetadata(false));

        public static readonly DependencyProperty LevelColorProperty =
            DependencyProperty.Register(nameof(LevelColor), typeof(Brush), typeof(ChemicalBucket), new PropertyMetadata(Brushes.Blue));

        public string BucketCode
        {
            get => (string)GetValue(BucketCodeProperty);
            set => SetValue(BucketCodeProperty, value);
        }

        public float LevelValue
        {
            get => (float)GetValue(LevelValueProperty);
            set => SetValue(LevelValueProperty, value);
        }

        public float MaxLevel
        {
            get => (float)GetValue(MaxLevelProperty);
            set => SetValue(MaxLevelProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public Brush LevelColor
        {
            get => (Brush)GetValue(LevelColorProperty);
            set => SetValue(LevelColorProperty, value);
        }

        private static void OnLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bucket = (ChemicalBucket)d;
            bucket.UpdateLevelColor();
        }

        private void UpdateLevelColor()
        {
            var percentage = MaxLevel > 0 ? LevelValue / MaxLevel : 0;

            if (percentage < 0.2f)
                LevelColor = new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red
            else if (percentage < 0.5f)
                LevelColor = new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange
            else if (percentage < 0.8f)
                LevelColor = new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Blue
            else
                LevelColor = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green
        }
    }
}
