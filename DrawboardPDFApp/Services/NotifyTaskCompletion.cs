using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Services
{
    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        public NotifyTaskCompletion(Task<TResult> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }

            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));

            if (task.IsCanceled)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
            }
            else if (task.IsFaulted)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Exception)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(InnerException)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
            }
            else
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Result)));
            }
        }

        public Task<TResult> Task { get; private set; }

        public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default;
        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsNotCompleted => !Task.IsCompleted;
        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException Exception => Task.Exception;
        public Exception InnerException => Exception?.InnerException;
        public string ErrorMessage => InnerException?.Message;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
