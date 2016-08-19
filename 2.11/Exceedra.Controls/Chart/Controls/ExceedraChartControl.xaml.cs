using System;
using Exceedra.Chart.ViewModels;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Exceedra.Chart.Model;
using Exceedra.Common.Utilities;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Controls.Legend;
using Telerik.Windows.Controls.Map;
using Telerik.Windows.Documents.FormatProviders.OpenXml.Docx;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Documents.Model;
using Point = System.Windows.Point;
using PointF = Telerik.Windows.Documents.Model.PointF;
using RectangleF = Telerik.Windows.Documents.Model.RectangleF;
using Size = System.Windows.Size;
using Exceedra.Common;

namespace Exceedra.Controls.Chart.Controls
{
    /// <summary>
    /// Interaction logic for ExceedraChartControl.xaml
    /// </summary>
    public partial class ExceedraChartControl : INotifyPropertyChanged
    {

        public ExceedraChartControl()
        {
            InitializeComponent();

            _filterDictionary = new Dictionary<string, string>()
            {
                {"PDF", "Adobe PDF Document (*.pdf)|*.pdf"},
                {"PNG", "PNG Images (*.png)|*.png"},
                {"WORD", "Word Documents (*.docx)|*.docx"},
                {"EXCEL", "Excel Worksheets (*.xlsx)|*.xlsx"}
            };

            _linearDictionary = new Dictionary<string, string>()
            {
                {"Point", "PointScatterSeriesDescriptorStyle"},
                {"Line", "LineScatterSeriesDescriptorStyle"},
                {"Spline", "SplineScatterSeriesDescriptorStyle"},
                {"Area", "AreaScatterSeriesDescriptorStyle"},
                {"Spline Area", "SplineAreaScatterSeriesDescriptorStyle"}
            };

            _categoricalDictionary = new Dictionary<string, string>()
            {
                {"Bar", "BarCategoricalSeriesDescriptorStyle"},
                {"Line","LineCategoricalSeriesDescriptorStyle"},
                {"Spline","SplineCategoricalSeriesDescriptorStyle"},
                {"Area","AreaCategoricalSeriesDescriptorStyle"},
                {"Spline Area","SplineAreaCategoricalSeriesDescriptorStyle"},
                {"Step Line","StepLineCategoricalSeriesDescriptorStyle"},
                {"Step Area","StepAreaCategoricalSeriesDescriptorStyle"}
            };
            
            _polarDictionary = new Dictionary<string, string>()
            {
                {"Point", "PointPolarSeriesDescriptorStyle"},
                {"Line", "LinePolarSeriesDescriptorStyle"},
                {"Area", "AreaPolarSeriesDescriptorStyle"}
            };

            _radarDictionary = new Dictionary<string, string>()
            {
                {"Point", "PointRadarSeriesDescriptorStyle"},
                {"Line", "LineRadarSeriesDescriptorStyle"},
                {"Area", "AreaRadarSeriesDescriptorStyle"}
            };

            _rangeDictionary = new Dictionary<string, string>()
            {
                {"Range", "RangeBarSeriesDescriptorStyle"},
            };

            _menuDictionary = new Dictionary<string, string>()
            {
                {"Categorical", "CategoricalMenu"},
                {"Linear", "LinearMenu"},
                {"Polar", "PolarMenu"},
                {"Radar", "RadarMenu"}
            };

        }

        private readonly Dictionary<string, string> _filterDictionary;
        private readonly Dictionary<string, string> _linearDictionary;
        private readonly Dictionary<string, string> _categoricalDictionary;
        private readonly Dictionary<string, string> _polarDictionary;
        private readonly Dictionary<string, string> _radarDictionary;
        private readonly Dictionary<string, string> _rangeDictionary;
        private readonly Dictionary<string, string> _menuDictionary;


        public static readonly DependencyProperty RecordSourceProperty =
            DependencyProperty.Register("RecordSource", typeof(RecordViewModel),
                typeof(ExceedraChartControl),
                new FrameworkPropertyMetadata() { PropertyChangedCallback = OnDataChanged, BindsTwoWayByDefault = true }
                );

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExceedraChartControl)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((RecordViewModel)e.NewValue != null)
            {
                RecordSource.Outliers += OutliersChanged;
                RecordSource.MetaData += MetaDataChanged;
                SetChartStuff(((RecordViewModel)e.NewValue).Chart);
                OutliersChanged();
            }
        }

        private void OutliersChanged()
        {
            SetOutlierAnnotations(RecordSource.Chart.Outliers);
        }

        private void MetaDataChanged()
        {
            if (RecordSource.VerticalLines != null)
                foreach (var line in RecordSource.VerticalLines)
                {
                    CartesianGridLineAnnotation annotation = new CartesianGridLineAnnotation();
                    annotation.Axis = CartesianChart.HorizontalAxis;
                    annotation.Stroke = new SolidColorBrush(Colors.DarkSlateBlue);
                    annotation.StrokeThickness = 1.5;
                    annotation.Value = line;
                    annotation.Tag = line;
                    CartesianChart.Annotations.Add(annotation);
                }

            if (RecordSource.MarkedAreas != null)
                foreach (var area in RecordSource.MarkedAreas)
                {
                    CartesianMarkedZoneAnnotation annotation = new CartesianMarkedZoneAnnotation();
                    annotation.HorizontalFrom = area.HorizontalFrom;
                    annotation.HorizontalTo = area.HorizontalTo;
                    //annotation.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(area.Fill);
                    annotation.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(area.Fill);
                    CartesianChart.Annotations.Add(annotation);
                }
        }

        private ObservableCollection<SingleSeries> AllSeries { get; set; }

        public LegendItemCollection LegendItems
        {
            get
            {
                if (AllSeries == null) return new LegendItemCollection();

                LegendItemCollection legendItems = new LegendItemCollection();

                foreach (var series in AllSeries)
                {
                    LegendItem legendItem = new LegendItem();

                    if (!string.IsNullOrEmpty(series.SeriesName))
                        legendItem.Title = series.SeriesName;

                    if (!string.IsNullOrEmpty(series.SeriesBrush))
                        legendItem.MarkerFill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(series.SeriesBrush));
                    else // If a serie has no colour specified take the respective one from the default palette. [I guess we use the Metro palette]
                    {
                        var indexOfSerie = AllSeries.IndexOf(series) % CartesianChart.Palette.GlobalEntries.Count;
                        legendItem.MarkerFill = CartesianChart.Palette.GlobalEntries[indexOfSerie].Fill;
                    }

                    legendItems.Add(legendItem);
                }

                return legendItems;
            }
        }

        private void SetChartStuff(Exceedra.Chart.Model.Chart chartDefinitions)
        {
            CartesianChart.Visibility = Visibility.Collapsed;
            PieChart.Visibility = Visibility.Collapsed;
            PolarChart.Visibility = Visibility.Collapsed;
            RadarChart.Visibility = Visibility.Collapsed;
            RangeBarChart.Visibility = Visibility.Collapsed;

            AllSeries = chartDefinitions.Series;
            PropertyChanged.Raise(this, "LegendItems");
            PropertyChanged.Raise(this, "AllSeries");

            try
            {
                switch (chartDefinitions.ChartType)
                {
                    case "Categorical":
                    case "Linear":
                        CartesianChart.Visibility = Visibility.Visible;
                        ResetZoom();
                        SetAxis(chartDefinitions);
                        SetSeriesDescriptor(chartDefinitions.ChartType, chartDefinitions.Series);
                        SetOutlierAnnotations(chartDefinitions.Outliers);
                        break;
                    case "Pie":
                        PieChart.Visibility = Visibility.Visible;
                        break;
                    case "Polar":
                        PolarChart.Visibility = Visibility.Visible;
                        SetSeriesDescriptor(chartDefinitions.ChartType, chartDefinitions.Series);
                        break;
                    case "Radar":
                        RadarChart.Visibility = Visibility.Visible;
                        SetSeriesDescriptor(chartDefinitions.ChartType, chartDefinitions.Series);
                        break;
                    case "Range":
                        RangeBarChart.Visibility = Visibility.Visible;
                        SetSeriesDescriptor(chartDefinitions.ChartType, chartDefinitions.Series);
                        break;
                }
            }
            catch (Exception e)
            {
                CartesianChart.Visibility = Visibility.Visible;
                //CartesianChart.EmptyContent = "No Series Selected";
            }


        }

        private void ResetZoom()
        {
            CartesianChart.Zoom = new Size(1, 1);
            CartesianChart.PanOffset = new Point(0, 0);
        }

        private void SetAxis(Exceedra.Chart.Model.Chart chartDefinitions)
        {
            chartDefinitions.XAxisType = chartDefinitions.XAxisType ?? "Categorical";
            if (chartDefinitions.XAxisType.Equals("Categorical"))
            {
                var categoricalXAxis = new CategoricalAxis();
                categoricalXAxis.Title = chartDefinitions.XAxisTitle;
                categoricalXAxis.LabelFitMode = AxisLabelFitMode.Rotate;

                if (!string.IsNullOrEmpty(chartDefinitions.XAxisBrush))
                    categoricalXAxis.ElementBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartDefinitions.XAxisBrush));

                CartesianChart.HorizontalAxis = categoricalXAxis;
            }
            else
            {
                var linearXAxis = new LinearAxis();
                linearXAxis.Title = chartDefinitions.XAxisTitle;
                //linearXAxis.Minimum = (double)chartDefinitions.XAxisMin;
                //linearXAxis.Maximum = (double)chartDefinitions.XAxisMax;
                linearXAxis.LabelFormat = "###,###,###,###.##";

                if (!string.IsNullOrEmpty(chartDefinitions.XAxisBrush))
                    linearXAxis.ElementBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartDefinitions.XAxisBrush));

                CartesianChart.HorizontalAxis = linearXAxis;
            }

            chartDefinitions.YAxisType = chartDefinitions.YAxisType ?? "Categorical";
            if (chartDefinitions.YAxisType.Equals("Categorical"))
            {
                var categoricalYAxis = new CategoricalAxis();
                categoricalYAxis.Title = chartDefinitions.YAxisTitle;

                if (!string.IsNullOrEmpty(chartDefinitions.YAxisBrush))
                    categoricalYAxis.ElementBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartDefinitions.YAxisBrush));

                CartesianChart.VerticalAxis = categoricalYAxis;
            }
            else
            {
                var linearYAxis = new LinearAxis();
                linearYAxis.Title = chartDefinitions.YAxisTitle;
                //linearYAxis.Minimum = (double)chartDefinitions.YAxisMin;
                //linearYAxis.Maximum = (double)chartDefinitions.YAxisMax;
                linearYAxis.LabelFormat = "###,###,###,###.##";

                if (!string.IsNullOrEmpty(chartDefinitions.YAxisBrush))
                    linearYAxis.ElementBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(chartDefinitions.YAxisBrush));

                CartesianChart.VerticalAxis = linearYAxis;
            }
        }

        /* This is terrible:
         * Was hard to configure in xaml so had to put it here.
         * Could maybe do something with item controls?
         * 
         * Basically this builds an array (SeriesDescriptors) which sets the binding values for each series.
         * We also set the collectionIndex which is the index of the series collection that this descriptor will look at. 
         * And then the Style is grabbed from the xaml static resources. 
         */
        private void SetSeriesDescriptor(string chartType, ObservableCollection<SingleSeries> allSeries)
        {
            CartesianChartSeriesProvider.SeriesDescriptors.Clear();
            PolarChartSeriesProvider.SeriesDescriptors.Clear();
            RadarChartSeriesProvider.SeriesDescriptors.Clear();
            SeriesMenu.Items.Clear();
            
            for (int i = 0; i < allSeries.Count; i++)
                switch (chartType)
                {
                    case "Categorical":
                        CategoricalSeriesDescriptor csd = new CategoricalSeriesDescriptor
                        {
                            ValuePath = "Y",
                            CategoryPath = "X",
                            ItemsSourcePath = "Datapoints",
                            CollectionIndex = i,
                            Style = GetSeriesType(chartType, allSeries[i].SeriesType)
                        };
                        CartesianChart.SeriesProvider.SeriesDescriptors.Add(csd);
                        AddToContextMenu(allSeries[i].SeriesName, i, chartType);
                        break;
                    case "Linear":
                        ScatterSeriesDescriptor ssd = new ScatterSeriesDescriptor
                        {
                            XValuePath = "X",
                            YValuePath = "Y",
                            ItemsSourcePath = "Datapoints",
                            CollectionIndex = i,
                            Style = GetSeriesType(chartType, allSeries[i].SeriesType)
                        };
                        CartesianChartSeriesProvider.SeriesDescriptors.Add(ssd);
                        AddToContextMenu(allSeries[i].SeriesName, i, chartType);
                        break;
                    case "Polar":
                        PolarSeriesDescriptor psd = new PolarSeriesDescriptor
                        {
                            AnglePath = "X",
                            ValuePath = "Y",
                            ItemsSourcePath = "Datapoints",
                            CollectionIndex = i,
                            Style = GetSeriesType(chartType, allSeries[i].SeriesType)
                        };
                        PolarChartSeriesProvider.SeriesDescriptors.Add(psd);
                        AddToContextMenu(allSeries[i].SeriesName, i, chartType);
                        break;
                    case "Radar":
                        RadarSeriesDescriptor rsd = new RadarSeriesDescriptor
                        {
                            ValuePath = "Y",
                            CategoryPath = "X",
                            ItemsSourcePath = "Datapoints",
                            CollectionIndex = i,
                            Style = GetSeriesType(chartType, allSeries[i].SeriesType)
                        };
                        RadarChartSeriesProvider.SeriesDescriptors.Add(rsd);
                        AddToContextMenu(allSeries[i].SeriesName, i, chartType);
                        break;
                    case "Range":
                        ObservableCollection<RangeDatapoint> data = new ObservableCollection<RangeDatapoint>();
                        allSeries[i].Datapoints.Do(p => data.Add((RangeDatapoint)p));
                        RangeBarSeries rbs = new RangeBarSeries
                        {
                            CategoryBinding = new PropertyNameDataPointBinding("X"),
                            LowBinding = new PropertyNameDataPointBinding("LowRange"),
                            HighBinding = new PropertyNameDataPointBinding("HighRange"),
                            ItemsSource = data
                        };
                        //rbs.DefaultVisualStyle = Resources["RangeStyle"] as Style;
                        RangeBarChart.Series.Add(rbs);
                        
                        break;

                    //TODO: Will need to update the Datapoint Model if we ever want to use this:
                    case "Ohlc":
                        OhlcSeriesDescriptor osd = new OhlcSeriesDescriptor();
                        //osd.OpenPath
                        //osd.HighPath
                        //osd.LowPath    
                        //osd.ClosePath
                        break;
                }
        }

        private void SetOutlierAnnotations(List<string> outlierValues)
        {
            if (CurrentOutlierValues != null && outlierValues != null && CurrentOutlierValues.ToHashSet().SetEquals(outlierValues)) return;

            CartesianChart.Annotations.RemoveAll();

            if(outlierValues != null)
            foreach (var outlier in outlierValues)
            {
                CartesianGridLineAnnotation annotation = new CartesianGridLineAnnotation();
                annotation.Axis = CartesianChart.HorizontalAxis;
                annotation.Stroke = new SolidColorBrush(Colors.DarkRed);
                annotation.StrokeThickness = 1;
                annotation.DashArray = new DoubleCollection {7,5};
                annotation.Value = outlier;
                annotation.Tag = outlier;
                CartesianChart.Annotations.Add(annotation);
            }
            CurrentOutlierValues = outlierValues;

            MetaDataChanged();
        }

        private List<string> CurrentOutlierValues { get; set; }

        //Add a new menu item for this series. Gives us access to individual series via the menu.
        private void AddToContextMenu(string seriesName, int index, string chartType)
        {
            var newSeriesItem = new RadMenuItem
            {
                Header = seriesName,
                Tag = index
            };

            CopyOfMenu(chartType, ref newSeriesItem);
            SeriesMenu.Items.Add(newSeriesItem);
        }

        /* We need to take a copy of the menu since using it directly from the xaml resource causes logical parent issues.
         * I.e. If one menu item is pointing at it, no one else can.
         */
        private void CopyOfMenu(string chartType, ref RadMenuItem newSeriesItem)
        {
            var copyOfResources = ((RadMenuItem)Resources[_menuDictionary[chartType]]);

            foreach (var item in copyOfResources.Items)
            {
                var itemCopy = new RadMenuItem
                {
                    Header = ((RadMenuItem)item).Header,
                    Name = ((RadMenuItem)item).Name,
                };

                itemCopy.Click += SeriesSelectItem_Click;

                newSeriesItem.Items.Add(itemCopy);
            }
        }

        private Style GetSeriesType(string chartType, string seriesType)
        {
            switch (chartType)
            {
                case "Categorical":
                    return Resources[_categoricalDictionary[seriesType]] as Style;
                case "Linear":
                    return Resources[_linearDictionary[seriesType]] as Style;
                case "Polar":
                    return Resources[_polarDictionary[seriesType]] as Style;
                case "Radar":
                    return Resources[_radarDictionary[seriesType]] as Style;
                case "Range":
                    return Resources[_rangeDictionary[seriesType]] as Style;
                default:
                    return new Style();
            }
        }

        public RecordViewModel RecordSource
        {
            get { return (RecordViewModel)GetValue(RecordSourceProperty); }

            set { SetValue(RecordSourceProperty, value); }
        }

        private void SeriesSelectItem_Click(object sender, RoutedEventArgs e)
        {
            RadMenuItem selectedItem = sender as RadMenuItem;

            if (selectedItem == null)
            {
                return;
            }

            RadMenuItem parent = (RadMenuItem)selectedItem.Parent;

            string selectedSeriesStyle = selectedItem.Name;
            int selectedSeriesId = Int32.Parse(parent.Tag.ToString());

            CartesianChart.SeriesProvider.SeriesDescriptors[selectedSeriesId].Style =
                Resources[selectedSeriesStyle] as Style;
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RadMenuItem selectedItem = sender as RadMenuItem;

            if (selectedItem == null)
            {
                return;
            }

            string selectedFormat = selectedItem.Header as string;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = _filterDictionary[selectedFormat];
            if (!(bool)dialog.ShowDialog())
            {
                return;
            }
            
            using (Stream fileStream = dialog.OpenFile())
            {
                if (selectedFormat == "PDF")
                {
                    RadDocument document = CreateDocument();
                    PrepareDocument(document);
                    PdfFormatProvider provider = new PdfFormatProvider();

                    provider.Export(document, fileStream);
                }
                else if (selectedFormat == "PNG")
                {
                    ExportPNGToImage(BaseGrid, fileStream);
                }
                else if (selectedFormat == "WORD")
                {
                    RadDocument document = CreateDocument();
                    PrepareDocument(document);
                    DocxFormatProvider provider = new DocxFormatProvider();
                    provider.Export(document, fileStream);
                }
                else if (selectedFormat == "EXCEL")
                {
                    Telerik.Windows.Media.Imaging.ExportExtensions.ExportToExcelMLImage(BaseGrid, fileStream);
                }
            }


        }

        private void ExportPNGToImage(FrameworkElement element, Stream stream)
        {
            Telerik.Windows.Media.Imaging.ExportExtensions.ExportToImage(element, stream, new PngBitmapEncoder());
        }

        private RadDocument CreateDocument()
        {
            RadDocument document = new RadDocument();

            CreateChartDocumentPart(document);

            //May need other options to export non grid/chart areas.

            return document;
        }

        private void CreateChartDocumentPart(RadDocument document)
        {
            Section section = new Section();
            Paragraph paragraph = new Paragraph();

            using (MemoryStream ms = new MemoryStream())
            {
                ExportPNGToImage(BaseGrid, ms);

                double imageWidth = BaseGrid.ActualWidth;
                double imageHeight = BaseGrid.ActualHeight;

                if (imageWidth > 625)
                {
                    imageWidth = 625;

                    imageHeight = this.BaseGrid.ActualHeight * imageWidth / BaseGrid.ActualWidth;
                }

                ImageInline image = new ImageInline(ms, new Size(imageWidth, imageHeight), "png");

                paragraph.Inlines.Add(image);
                section.Blocks.Add(paragraph);
                document.Sections.Add(section);
            }
        }

        private void PrepareDocument(RadDocument document)
        {
            document.LayoutMode = DocumentLayoutMode.Paged;
            document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
            document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));
        }

        private void ChartSelectionBehavior_SelectionChanged(object sender, ChartSelectionChangedEventArgs e)
        {
            //HyperLinkHandler.Invoke(sender, new RoutedEventArgs());

            var clickedPoint = e.AddedPoints.FirstOrDefault();
            if (clickedPoint != null)
            {
                var idx = ((Datapoint)clickedPoint.DataItem).ID;
                var navType = ((Datapoint)clickedPoint.DataItem).NavigationType;

                if (idx == null || navType == null) return;

                var parentPage = VisualTreeHelper.GetParent(this);
                while (!(parentPage is Page))
                {
                    parentPage = VisualTreeHelper.GetParent(parentPage);
                }

                //Get the linkHandler method used by the parent ROB List page
                var method = parentPage.GetType().GetMethod("DataPointClicked");

                if (method == null)
                {
                    parentPage = VisualTreeHelper.GetParent(parentPage);
                    while (!(parentPage is Page))
                    {
                        parentPage = VisualTreeHelper.GetParent(parentPage);
                    }
                    method = parentPage.GetType().GetMethod("DataPointClicked");
                }

                object[] param = { navType, idx };
                method.Invoke(parentPage, param);
            }
        }

        public static readonly DependencyProperty HyperLinkHandlerProperty =
            DependencyProperty.Register("HyperLinkHandler",
                                        typeof(RoutedEventHandler),
                                        typeof(ExceedraChartControl),
                                        null);

        public RoutedEventHandler HyperLinkHandler
        {
            get { return (RoutedEventHandler)GetValue(HyperLinkHandlerProperty); }
            set
            {
                if (value != null)
                {
                    SetValue(HyperLinkHandlerProperty, value);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void LegendItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            
            //StackPanel legendItem = (StackPanel) sender;
            //System.Windows.Shapes.Path colour = (System.Windows.Shapes.Path)legendItem.Children[0];
            //TextBlock textBlock = (TextBlock)legendItem.Children[1];

            //var grey = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF808080"));
            //var seriesIndex = AllSeries.IndexOf(AllSeries.FirstOrDefault(series => series.SeriesName == textBlock.Text));


            //if (legendItem.Opacity == 1)
            //{
            //    CartesianChart.SeriesProvider.SeriesDescriptors[seriesIndex].ItemsSourcePath = null;
            //    legendItem.Opacity = 0.5;
            //    //if(colour.Tag == null) colour.Tag = colour.Fill;
            //    //colour.Fill = grey;
            //    //textBlock.Foreground = grey;
            //}
            //else
            //{
            //    legendItem.Opacity = 1;
            //    //colour.Fill = (System.Windows.Media.Brush)colour.Tag;
            //    //textBlock.Foreground = Brushes.Black;
            //    CartesianChart.SeriesProvider.SeriesDescriptors[seriesIndex].ItemsSourcePath = "Datapoints";
            //}

        }

        private void LegendItem_MouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
                {
                    var legend = (RadLegend) sender;
                    var clickedSeries = legend.Items.First(i => i.IsHovered);

                    var seriesIndex = AllSeries.IndexOf(AllSeries.FirstOrDefault(series => series.SeriesName == clickedSeries.Title));

                    CartesianChart.SeriesProvider.SeriesDescriptors[seriesIndex].ItemsSourcePath = CartesianChart.SeriesProvider.SeriesDescriptors[seriesIndex].ItemsSourcePath == null ? "Datapoints" : null;
                }

        public Grid GetBaseGrid()
        {
            return BaseGrid;
        }

        public IEnumerable<LowHighMeasurementWeatherData> testData { get; set; }


        private static IEnumerable<LowHighMeasurementWeatherData> GetTemperatureData()
        {
            return new List<LowHighMeasurementWeatherData>()
			{
				new LowHighMeasurementWeatherData(new DateTime(2011, 1, 1), -14, 12),
				new LowHighMeasurementWeatherData(new DateTime(2011, 2, 1), -9, 19),
				new LowHighMeasurementWeatherData(new DateTime(2011, 3, 1), -7, 25),
				new LowHighMeasurementWeatherData(new DateTime(2011, 4, 1), 2, 28),
				new LowHighMeasurementWeatherData(new DateTime(2011, 5, 1), 8, 32),
				new LowHighMeasurementWeatherData(new DateTime(2011, 6, 1), 13, 35),
				new LowHighMeasurementWeatherData(new DateTime(2011, 7, 1), 17, 40),
				new LowHighMeasurementWeatherData(new DateTime(2011, 8, 1), 15, 34),
				new LowHighMeasurementWeatherData(new DateTime(2011, 9, 1), 11, 30),
				new LowHighMeasurementWeatherData(new DateTime(2011, 10, 1), 1, 29),
				new LowHighMeasurementWeatherData(new DateTime(2011, 11, 1), 2, 21),
				new LowHighMeasurementWeatherData(new DateTime(2011, 12, 1), -1, 17),
				new LowHighMeasurementWeatherData(new DateTime(2012, 1, 1), -11, 17),
				new LowHighMeasurementWeatherData(new DateTime(2012, 2, 1), -7, 17),
				new LowHighMeasurementWeatherData(new DateTime(2012, 3, 1), -4, 25),
				new LowHighMeasurementWeatherData(new DateTime(2012, 4, 1), 3, 31),
				new LowHighMeasurementWeatherData(new DateTime(2012, 5, 1), 9, 32),
				new LowHighMeasurementWeatherData(new DateTime(2012, 6, 1), 11, 34),
				new LowHighMeasurementWeatherData(new DateTime(2012, 7, 1), 16, 38),
				new LowHighMeasurementWeatherData(new DateTime(2012, 8, 1), 16, 33),
				new LowHighMeasurementWeatherData(new DateTime(2012, 9, 1), 12, 33),
				new LowHighMeasurementWeatherData(new DateTime(2012, 10, 1), 3, 26),
			};
        }

        private void ToggleLegend(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            LegendVisibilityIcon.Visibility = CartesianLegend.Visibility = CartesianLegend.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public class LowHighMeasurementWeatherData
    {
        public string Date { get; set; }
        public double LowMeasurement { get; set; }
        public double HighMeasurement { get; set; }

        public LowHighMeasurementWeatherData(DateTime date, double lowMeasurement, double highMeasurement)
        {
            this.Date = date.ToShortDateString();
            this.LowMeasurement = lowMeasurement;
            this.HighMeasurement = highMeasurement;
        }
    }
}
