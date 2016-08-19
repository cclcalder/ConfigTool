using System.Windows;
using System.Windows.Controls;

namespace Exceedra.Buttons
{
    /// <summary>
    /// The Cross Button is a very simple version of the button that displays as a discrete cross,
    /// similar to the buttons at the top of Google Chrome's tabs.
    /// </summary>
    public class CrossButton : Button
    {
        /// <summary>
        /// Initializes the <see cref="CrossButton"/> class.
        /// </summary>
        static CrossButton()
        {
            //  Set the style key, so that our control template is used.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CrossButton), new FrameworkPropertyMetadata(typeof(CrossButton)));
        }
    }

    public class ArrowButton : Button
    {
        /// <summary>
        /// Initializes the <see cref="ArrowButton"/> class.
        /// </summary>
        static ArrowButton()
        {
            //  Set the style key, so that our control template is used.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ArrowButton), new FrameworkPropertyMetadata(typeof(ArrowButton)));
        }
    }

    public class LeftArrowButton : Button
    {
        /// <summary>
        /// Initializes the <see cref="LeftArrowButton"/> class.
        /// </summary>
        static LeftArrowButton()
        {
            //  Set the style key, so that our control template is used.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LeftArrowButton), new FrameworkPropertyMetadata(typeof(LeftArrowButton)));
        }
    }

    public class PlusButton : Button
    {
        /// <summary>
        /// Initializes the <see cref="PlusButton"/> class.
        /// </summary>
        static PlusButton()
        {
            //  Set the style key, so that our control template is used.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlusButton), new FrameworkPropertyMetadata(typeof(PlusButton)));
        }
    }

    public class SearchButton : Button
    {
        /// <summary>
        /// Initializes the <see cref="SearchButton"/> class.
        /// </summary>
        static SearchButton()
        {
            //  Set the style key, so that our control template is used.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchButton), new FrameworkPropertyMetadata(typeof(SearchButton)));
        }
    }
}
