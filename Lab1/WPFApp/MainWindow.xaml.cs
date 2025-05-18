using System;
using System.Windows;
using BusinessObjects;
using System.Windows.Controls;
using Services;

namespace ProductMangement
{
    /// <summary> Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        private readonly IProductService iProductService;
        private readonly ICategoryService iCategoryService;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                iProductService = new ProductService();
                iCategoryService = new CategoryService();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize services: {ex.Message}", "Initialization Error");
                Close();
            }
        }

        public void LoadCategoryList()
        {
            try
            {
                var catList = iCategoryService.GetCategories();
                if (catList == null)
                {
                    MessageBox.Show("Category list is null.", "Error");
                    return;
                }
                cboCategory.ItemsSource = catList;
                cboCategory.DisplayMemberPath = "CategoryName";
                cboCategory.SelectedValuePath = "CategoryId";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error on load list of categories");
            }
        }

        public void LoadProductList()
        {
            try
            {
                var catList = iCategoryService.GetCategories();
                if (catList == null)
                {
                    MessageBox.Show("Category list is null.", "Error");
                    return;
                }

                var productList = iProductService.GetProducts();
                if (productList == null)
                {
                    MessageBox.Show("Product list is null.", "Error");
                    return;
                }

                foreach (var item in productList)
                {
                    var catName = catList.Find(c => c.CategoryId == item.CategoryId);
                    if (catName != null)
                    {
                        if (item.Category == null)
                        {
                            item.Category = new Category();
                        }
                        item.Category.CategoryName = catName.CategoryName;
                        item.Category.CategoryId = catName.CategoryId; // Optional: Keep IDs in sync
                    }
                    else
                    {
                        MessageBox.Show($"Category with ID {item.CategoryId} not found!");
                    }
                }
                dgData.ItemsSource = productList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error on load list of products");
            }
            finally
            {
                resetInput();
                dgData.Items.Refresh();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategoryList();
            LoadProductList();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Product product = new Product();
                //product.ProductName = txtProductName.Text;
                //product.UnitPrice = Decimal.Parse(txtPrice.Text);
                //product.UnitsInStock = short.Parse(txtUnitsInStock.Text);
                //product.CategoryId = Int32.Parse(cboCategory.SelectedValue.ToString());
                //iProductService.SaveProduct(product);
                if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                    !decimal.TryParse(txtPrice.Text, out decimal unitPrice) ||
                    !short.TryParse(txtUnitsInStock.Text, out short unitsInStock) ||
                    cboCategory.SelectedValue == null ||
                    !int.TryParse(cboCategory.SelectedValue.ToString(), out int categoryId))
                {
                    MessageBox.Show("Please fill in all fields with valid data.", "Input Error");
                    return;
                }

                Product product = new Product
                {
                    ProductName = txtProductName.Text,
                    UnitPrice = unitPrice,
                    UnitsInStock = unitsInStock,
                    CategoryId = categoryId
                };
                iProductService.SaveProduct(product);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error creating product");
            }
            finally
            {
                LoadProductList();
            }
        }
        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DataGrid dataGrid = sender as DataGrid;
            //DataGridRow row = (DataGridRow) dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
            //DataGridCell RowColumn = dataGrid.Columns[0].GetCellContent(row).Parent as DataGridCell;
            //string id = ( (TextBlock) RowColumn.Content ).Text;
            //Product product = iProductService.GetProductById(Int32.Parse(id));
            //txtProductID.Text = product.ProductId.ToString();
            //txtProductName.Text = product.ProductName;
            //txtPrice.Text = product.UnitPrice.ToString();
            //txtUnitsInStock.Text = product.UnitsInStock.ToString();
            //cboCategory.SelectedValue = product.CategoryId;

            if (!( sender is DataGrid dataGrid ) || dataGrid.SelectedIndex < 0)
                return;

            try
            {
                var row = (DataGridRow) dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                if (row == null)
                    return;

                var cellContent = dataGrid.Columns[0].GetCellContent(row);
                if (!( cellContent?.Parent is DataGridCell cell ) || !( cell.Content is TextBlock textBlock ) || string.IsNullOrEmpty(textBlock.Text))
                    return;

                if (!int.TryParse(textBlock.Text, out int id))
                    return;

                var product = iProductService.GetProductById(id);
                if (product == null)
                {
                    MessageBox.Show("Selected product not found.", "Error");
                    return;
                }

                txtProductID.Text = product.ProductId.ToString();
                txtProductName.Text = product.ProductName ?? "";
                txtPrice.Text = product.UnitPrice.ToString();
                txtUnitsInStock.Text = product.UnitsInStock.ToString();
                cboCategory.SelectedValue = product.CategoryId;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product details: {ex.Message}", "Error");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtProductID.Text.Length > 0)
                {
                    Product product = new Product();
                    product.ProductId = Int32.Parse(txtProductID.Text);
                    product.ProductName = txtProductName.Text;
                    product.UnitPrice = Decimal.Parse(txtPrice.Text);
                    product.UnitsInStock = short.Parse(txtUnitsInStock.Text);
                    product.CategoryId = Int32.Parse(cboCategory.SelectedValue.ToString());
                    iProductService.UpdateProduct(product);
                }
                else
                {
                    MessageBox.Show("You must select a Product !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error updating product");
            }
            finally
            {
                LoadProductList();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtProductID.Text.Length > 0)
                {
                    Product product = new Product();
                    product.ProductId = Int32.Parse(txtProductID.Text);
                    product.ProductName = txtProductName.Text;
                    product.UnitPrice = Decimal.Parse(txtPrice.Text);
                    product.UnitsInStock = short.Parse(txtUnitsInStock.Text);
                    product.CategoryId = Int32.Parse(cboCategory.SelectedValue.ToString());
                    iProductService.DeleteProduct(product);
                }
                else
                {
                    MessageBox.Show("You must select a Product !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error deleting product");
            }
            finally
            {
                LoadProductList();
            }
        }

        private void resetInput()
        {
            txtProductID.Text = "";
            txtProductName.Text = "";
            txtPrice.Text = "";
            txtUnitsInStock.Text = "";
            cboCategory.SelectedValue = 0;
        }

    }
}