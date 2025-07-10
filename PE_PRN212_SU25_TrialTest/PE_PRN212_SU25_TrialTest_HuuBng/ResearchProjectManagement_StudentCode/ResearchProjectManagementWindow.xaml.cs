using Objects;
using Services;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ResearchProjectManagement_NoXXXXX
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ResearchProjectManagementWindow : Window
    {
        private readonly ResearchProjectService _service;
        private readonly int _role;
        private bool _isMember;
        public ResearchProjectManagementWindow(int Role)
        {
            InitializeComponent();

            _service = new();
            _role = Role;

            ApplyAuthorization();
            LoadResearchers();
            LoadProjects();

            if (_isMember)
            {
                MessageBox.Show("You have no permission to access this function!");
            }

            dgProjects.SelectionChanged += DgProjects_SelectionChanged;
        }

        public void ApplyAuthorization()
        {
            switch (_role)
            {
                case 1: // Admin
                    // Default true for everything
                    break;
                case 2: // Manager
                    BtnDelete.IsEnabled = false;
                    break;
                case 3: // Staff
                    BtnCreate.IsEnabled = false;
                    BtnUpdate.IsEnabled = false;
                    BtnDelete.IsEnabled = false;
                    break;
                default:
                    BtnCreate.IsEnabled = false;
                    BtnUpdate.IsEnabled = false;
                    BtnDelete.IsEnabled = false;
                    BtnSearch.IsEnabled = false;
                    _isMember = true;
                    break;
            }
        }

        public void LoadProjects()
        {
            if (_isMember) return;

            dgProjects.ItemsSource = _service.GetResearchProjects();
        }

        public void LoadResearchers()
        {
            if (_isMember) return;
            cbLeadResearcher.ItemsSource = _service.GetResearchers();
            cbLeadResearcher.DisplayMemberPath = "FullName";
            cbLeadResearcher.SelectedValuePath = "ResearcherID";
        }

        private void DgProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProjects.SelectedItem is ResearchProject p)
            {
                txtProjectTitle.Text = p.ProjectTitle;
                txtResearchField.Text = p.ResearchField;
                dpStartDate.SelectedDate = p.StartDate;
                dpEndDate.SelectedDate = p.EndDate;
                cbLeadResearcher.SelectedValue = p.LeadResearcherID;
                txtBudget.Text = p.Budget.ToString();
            }
            else
            {
                ClearInputFields();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            var projects = _service.SearchProjects(keyword);
            dgProjects.ItemsSource = projects;
        }

        private void ClearInputFields()
        {
            txtProjectTitle.Text = "";
            txtResearchField.Text = "";
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            cbLeadResearcher.SelectedIndex = -1;
            txtBudget.Text = "";
            dgProjects.SelectedIndex = -1;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string title = txtProjectTitle.Text.Trim();
                string field = txtResearchField.Text.Trim();
                DateTime? start = dpStartDate.SelectedDate;
                DateTime? end = dpEndDate.SelectedDate;
                var researcherId = cbLeadResearcher.SelectedValue;
                string budgetText = txtBudget.Text.Trim();

                // Validation
                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(field) ||
                    !start.HasValue || !end.HasValue || researcherId == null || string.IsNullOrEmpty(budgetText))
                {
                    MessageBox.Show("All fields are required.");
                    return;
                }
                if (start >= end)
                {
                    MessageBox.Show("Start Date must be earlier than End Date.");
                    return;
                }
                if (title.Length < 5 || title.Length > 100)
                {
                    MessageBox.Show("Project Title must be between 5 and 100 characters.");
                    return;
                }
                Regex regex = new Regex(@"^([A-Z1-9][a-zA-Z0-9]*\s?)+$");
                if (!regex.IsMatch(title))
                {
                    MessageBox.Show("Each word must start with capital letter or digit, no special chars.");
                    return;
                }
                if (!decimal.TryParse(budgetText, out decimal budget) || budget < 0)
                {
                    MessageBox.Show("Budget must be a valid non-negative number.");
                    return;
                }

                // Create
                var project = new ResearchProject
                {
                    ProjectTitle = title,
                    ResearchField = field,
                    StartDate = start.Value,
                    EndDate = end.Value,
                    LeadResearcherID = (int)researcherId,
                    Budget = budget
                };

                _service.CreateResearchProject(project);
                LoadProjects();
                ClearInputFields();

                MessageBox.Show("Project created successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }

        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgProjects.SelectedItem is ResearchProject selected)
            {
                try
                {
                    string title = txtProjectTitle.Text.Trim();
                    string field = txtResearchField.Text.Trim();
                    DateTime? start = dpStartDate.SelectedDate;
                    DateTime? end = dpEndDate.SelectedDate;
                    var researcherId = cbLeadResearcher.SelectedValue;
                    string budgetText = txtBudget.Text.Trim();

                    if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(field) ||
                        !start.HasValue || !end.HasValue || researcherId == null || string.IsNullOrEmpty(budgetText))
                    {
                        MessageBox.Show("All fields are required."); return;
                    }
                    if (start >= end) { MessageBox.Show("Start Date must be earlier than End Date."); return; }
                    if (title.Length < 5 || title.Length > 100) { MessageBox.Show("Title must be 5-100 chars."); return; }
                    Regex regex = new Regex(@"^([A-Z1-9][a-zA-Z0-9]*\s?)+$");
                    if (!regex.IsMatch(title)) { MessageBox.Show("Each word must start with capital/digit, no special chars."); return; }
                    if (!decimal.TryParse(budgetText, out decimal budget) || budget < 0)
                    {
                        MessageBox.Show("Budget must be non-negative number."); return;
                    }

                    selected.ProjectTitle = title;
                    selected.ResearchField = field;
                    selected.StartDate = start.Value;
                    selected.EndDate = end.Value;
                    selected.LeadResearcherID = (int)researcherId;
                    selected.Budget = budget;

                    _service.UpdateResearchProject(selected);
                    LoadProjects();
                    ClearInputFields();

                    MessageBox.Show("Project updated successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating project: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a project to update.");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgProjects.SelectedItem is ResearchProject selected)
            {
                var confirm = MessageBox.Show($"Delete project '{selected.ProjectTitle}'?", "Confirm", MessageBoxButton.YesNo);
                if (confirm == MessageBoxResult.Yes)
                {
                    _service.DeleteResearchProject(selected.ProjectID);
                    LoadProjects();
                    ClearInputFields();
                    MessageBox.Show("Project deleted successfully!");
                }
            }
            else
            {
                MessageBox.Show("Please select a project to delete.");
            }
        }
    }
}
