using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Arushanyan_LB4.Data;

namespace Arushanyan_LB4.Forms
{
    public class MainForm : Form
    {
        private readonly UserDataContext _db = new UserDataContext();
        private readonly BindingSource _usersSource = new BindingSource();
        private readonly BindingSource _rolesSource = new BindingSource();
        private readonly BindingNavigator _navigator;
        private readonly DataGridView _grid;
        private readonly Button _saveButton;
        private readonly Label _status;

        public MainForm()
        {
            Text = "Arushanyan-LB4 — Работа с BindingNavigator";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(760, 440);
            Size = new Size(920, 560);

            _navigator = new BindingNavigator(true) { Dock = DockStyle.Top, BindingSource = _usersSource };
            _navigator.AddNewItem = null;
            _navigator.DeleteItem = null;
            var addButton = new ToolStripButton("Добавить") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var deleteButton = new ToolStripButton("Удалить") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            _navigator.Items.Add(new ToolStripSeparator());
            _navigator.Items.Add(addButton);
            _navigator.Items.Add(deleteButton);
            addButton.Click += AddUser;
            deleteButton.Click += DeleteUser;

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill, AutoGenerateColumns = false, AllowUserToAddRows = false,
                AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
                DataSource = _usersSource, RowHeadersVisible = false
            };
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID", HeaderText = "ID", ReadOnly = true, FillWeight = 35 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "First_Name", HeaderText = "Имя", FillWeight = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Last_Name", HeaderText = "Фамилия", FillWeight = 100 });
            _grid.Columns.Add(new DataGridViewComboBoxColumn { DataPropertyName = "Role_ID", HeaderText = "Роль", DataSource = _rolesSource, DisplayMember = "Role_Name", ValueMember = "ID", FillWeight = 90, FlatStyle = FlatStyle.Flat });
            _grid.DataError += (s, e) => { e.ThrowException = false; };

            _saveButton = new Button { Text = "Сохранить изменения", Dock = DockStyle.Right, Width = 170, Height = 34 };
            _saveButton.Click += (s, e) => SaveData();
            _status = new Label { Text = "Готово", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(8, 0, 0, 0) };
            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 44, Padding = new Padding(6) };
            bottom.Controls.Add(_status);
            bottom.Controls.Add(_saveButton);
            Controls.Add(_grid);
            Controls.Add(bottom);
            Controls.Add(_navigator);
            Load += (s, e) => StartLoadData();
            FormClosed += (s, e) => _db.Dispose();
        }

        private void StartLoadData()
        {
            try
            {
                _db.Roles.Load();
                _db.Users.Include(u => u.Role).Load();
                _rolesSource.DataSource = _db.Roles.Local.ToBindingList();
                _usersSource.DataSource = _db.Users.Local.ToBindingList();
                _status.Text = "Данные загружены. Записей: " + _usersSource.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить данные. Проверьте SQL Server и базу DB_Users.\n\n" + ex.Message,
                    "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _status.Text = "Ошибка загрузки данных";
            }
        }

        private void AddUser(object sender, EventArgs e)
        {
            if (_db.Roles.Local.Count == 0)
            {
                MessageBox.Show("Сначала добавьте хотя бы одну запись в таблицу Roles.", "Нет ролей", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var user = _db.Users.Create();
            user.First_Name = "";
            user.Last_Name = "";
            user.Role_ID = _db.Roles.Local.First().ID;
            _db.Users.Add(user);
            _usersSource.Position = _usersSource.IndexOf(user);
            _grid.BeginEdit(true);
            _status.Text = "Добавлена новая запись — заполните поля и нажмите «Сохранить изменения»";
        }

        private void DeleteUser(object sender, EventArgs e)
        {
            var user = _usersSource.Current as User;
            if (user == null) return;
            if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            _db.Users.Remove(user);
            _usersSource.RemoveCurrent();
            SaveData();
        }

        private void SaveData()
        {
            try
            {
                Validate();
                _grid.EndEdit();
                _usersSource.EndEdit();
                _db.SaveChanges();
                _status.Text = "Изменения сохранены. Записей: " + _usersSource.Count;
                MessageBox.Show("Изменения успешно сохранены в базе данных.", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось сохранить изменения. Проверьте заполнение полей и выбранную роль.\n\n" + ex.Message,
                    "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _status.Text = "Ошибка сохранения";
            }
        }
    }
}
