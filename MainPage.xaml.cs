using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace ToDo_App
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<ToDoItem> tasks = new ObservableCollection<ToDoItem>();

        public MainPage()
        {
            InitializeComponent();
            LoadTasks(); // Загружаем задачи из локального хранилища
            TasksCollectionView.ItemsSource = tasks;

            // Подписка на изменения в коллекции для автообновления статусов
            foreach (var task in tasks)
            {
                task.PropertyChanged += Task_PropertyChanged;
            }
        }

        // Добавление новой задачи
        private void AddTaskButton_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewTaskEntry.Text))
            {
                var newTask = new ToDoItem { Text = NewTaskEntry.Text, IsCompleted = false };
                newTask.PropertyChanged += Task_PropertyChanged; // Подписка на изменения задачи
                tasks.Add(newTask);
                NewTaskEntry.Text = string.Empty;
                SaveTasks(); // Сохраняем обновлённый список задач
            }
        }

        // Установка дедлайна для новой задачи
        private async void PickDeadlineButton_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayDatePicker();
            if (result.HasValue)
            {
                var newTask = new ToDoItem
                {
                    Text = NewTaskEntry.Text,
                    Deadline = result,
                    IsCompleted = false
                };

                newTask.PropertyChanged += Task_PropertyChanged;
                tasks.Add(newTask);
                SaveTasks();
            }
        }

        // Удаление задачи
        private void DeleteTaskButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var toDoItem = button?.CommandParameter as ToDoItem;
            if (toDoItem != null)
            {
                tasks.Remove(toDoItem);
                SaveTasks(); // Сохраняем обновлённый список задач
            }
        }

        // Редактирование задачи
        private async void EditTaskButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var toDoItem = button?.CommandParameter as ToDoItem;
            if (toDoItem != null)
            {
                string result = await DisplayPromptAsync("Редактировать задачу", "Измените текст задачи:", initialValue: toDoItem.Text);
                if (!string.IsNullOrEmpty(result))
                {
                    toDoItem.Text = result;

                    var newDeadline = await DisplayDatePicker();
                    if (newDeadline.HasValue)
                        toDoItem.Deadline = newDeadline;

                    SaveTasks(); // Сохраняем изменения
                }
            }
        }

        private async Task<DateTime?> DisplayDatePicker()
        {
            var date = await DisplayPromptAsync("Выберите дату", "Введите дату в формате: ДД.ММ.ГГГГ", placeholder: "31.12.2024");
            var time = await DisplayPromptAsync("Выберите время", "Введите время в формате: ЧЧ:ММ", placeholder: "23:59");

            if (DateTime.TryParse($"{date} {time}", out DateTime deadline))
                return deadline;

            return null;
        }

        // Обработчик изменения задачи
        private void Task_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ToDoItem.IsCompleted) || e.PropertyName == nameof(ToDoItem.Text) || e.PropertyName == nameof(ToDoItem.Deadline))
            {
                SaveTasks(); // Сохраняем задачи при изменении
            }
        }

        // Сохранение задач
        private void SaveTasks()
        {
            var serializedTasks = JsonSerializer.Serialize(tasks);
            Preferences.Set("SavedTasks", serializedTasks);
        }

        // Загрузка задач
        private void LoadTasks()
        {
            var serializedTasks = Preferences.Get("SavedTasks", string.Empty);
            if (!string.IsNullOrWhiteSpace(serializedTasks))
            {
                var loadedTasks = JsonSerializer.Deserialize<ObservableCollection<ToDoItem>>(serializedTasks);
                if (loadedTasks != null)
                {
                    tasks = loadedTasks;
                    foreach (var task in tasks)
                    {
                        task.PropertyChanged += Task_PropertyChanged;
                    }
                }
            }
        }
    }
}