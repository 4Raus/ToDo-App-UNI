using System;
using System.ComponentModel;
using Microsoft.Maui.Graphics;

namespace ToDo_App
{
    public class ToDoItem : INotifyPropertyChanged
    {
        private string text;
        private bool isCompleted;
        private DateTime? deadline;

        public string Text
        {
            get => text;
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                if (isCompleted != value)
                {
                    isCompleted = value;
                    OnPropertyChanged(nameof(IsCompleted));
                    OnPropertyChanged(nameof(TextColor));
                    OnPropertyChanged(nameof(DeadlineColor));
                }
            }
        }

        public DateTime? Deadline
        {
            get => deadline;
            set
            {
                if (deadline != value)
                {
                    deadline = value;
                    OnPropertyChanged(nameof(Deadline));
                    OnPropertyChanged(nameof(TextColor));
                    OnPropertyChanged(nameof(DeadlineColor));
                }
            }
        }

        // Цвет текста задачи
        public Color TextColor
        {
            get
            {
                if (IsCompleted)
                    return Colors.Green;
                if (Deadline.HasValue && Deadline < DateTime.Now)
                    return Colors.Red;
                return Colors.White;
            }
        }

        // Цвет времени дедлайна
        public Color DeadlineColor
        {
            get
            {
                if (IsCompleted)
                    return Colors.Green; // Зеленый, если задача выполнена
                if (Deadline.HasValue && Deadline < DateTime.Now)
                    return Colors.Red; // Красный, если дедлайн пропущен
                return Colors.Gray; // Серый в остальных случаях
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}