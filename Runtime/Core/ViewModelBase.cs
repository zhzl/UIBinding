using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UIBinding
{
#if !DISABLE_UIBINDING_INJECT

    /// <summary>
    /// ViewModel基类
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected object[] notifyFields;
        protected string[] propertyNames;

        public event PropertyChangedEventHandler OnPropertyChanged;

        protected void Init(int count)
        {
            if (count <= 0) return;
            notifyFields = new object[count];
            propertyNames = new string[count];
        }

        protected T Get<T>(int index)
        {
            var value = notifyFields[index];
            if (value != null)
            {
                if (value is Boxed<T> boxed)
                {
                    return boxed.value;
                }
                else
                {
                    return (T)value;
                }
            }
            return default(T);
        }

        protected bool Set<T>(T newValue, int index, string propertyName)
        {
            var oldValue = notifyFields[index];
            if (oldValue == null)
            {
                propertyNames[index] = propertyName;

                if (typeof(T).IsValueType)
                {
                    var boxed = new Boxed<T>(newValue);
                    notifyFields[index] = boxed;
                    OnPropertyChanged?.Invoke(this, index, propertyName, boxed);
                    return true;
                }
                else
                {
                    notifyFields[index] = newValue;
                    OnPropertyChanged?.Invoke(this, index, propertyName, newValue);
                    return true;
                }
            }
            else
            {
                if (oldValue is Boxed<T> boxed)
                {
                    if (!EqualityComparer<T>.Default.Equals(boxed.value, newValue))
                    {
                        boxed.value = newValue;
                        OnPropertyChanged?.Invoke(this, index, propertyName, boxed);
                        return true;
                    }
                }
                else
                {
                    if (!EqualityComparer<T>.Default.Equals((T)oldValue, newValue))
                    {
                        notifyFields[index] = newValue;
                        OnPropertyChanged?.Invoke(this, index, propertyName, newValue);
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual void Reset()
        {
            OnPropertyChanged = null;
            for (int i = 0; i < notifyFields.Length; i++)
            {
                notifyFields[i] = null;
            }
        }

        public void NotifyAll()
        {
            for (int i = 0; i < notifyFields.Length; i++)
            {
                if (notifyFields[i] != null)
                {
                    OnPropertyChanged?.Invoke(this, i, propertyNames[i], notifyFields[i]);
                }
            }
        }
    }
#else
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected object[] notifyFields;
        protected string[] propertyNames;

        public event PropertyChangedEventHandler OnPropertyChanged;

        private readonly Dictionary<string, int> propertyMap = new Dictionary<string, int>(StringComparer.Ordinal);

        public ViewModelBase()
        {
            var properties = this.GetType().GetProperties();
            var notifyCount = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (!property.HasAttribute<NotifyAttribute>())
                {
                    continue;
                }
                propertyMap[property.Name] = notifyCount;
                notifyCount++;
            }

            Init(notifyCount);
        }

        protected void Init(int count)
        {
            if (count <= 0) return;
            notifyFields = new object[count];
            propertyNames = new string[count];
        }

        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            if (!propertyMap.TryGetValue(propertyName, out int index))
            {
                return default(T);
            }

            var value = notifyFields[index];
            if (value != null)
            {
                if (value is Boxed<T> boxed)
                {
                    return boxed.value;
                }
                else
                {
                    return (T)value;
                }
            }
            return default(T);
        }

        protected bool Set<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!propertyMap.TryGetValue(propertyName, out int index))
            {
                return false;
            }

            var oldValue = notifyFields[index];
            if (oldValue == null)
            {
                propertyNames[index] = propertyName;

                if (typeof(T).IsValueType)
                {
                    var boxed = new Boxed<T>(newValue);
                    notifyFields[index] = boxed;
                    OnPropertyChanged?.Invoke(this, index, propertyName, boxed);
                    return true;
                }
                else
                {
                    notifyFields[index] = newValue;
                    OnPropertyChanged?.Invoke(this, index, propertyName, newValue);
                    return true;
                }
            }
            else
            {
                if (oldValue is Boxed<T> boxed)
                {
                    if (!EqualityComparer<T>.Default.Equals(boxed.value, newValue))
                    {
                        boxed.value = newValue;
                        OnPropertyChanged?.Invoke(this, index, propertyName, boxed);
                        return true;
                    }
                }
                else
                {
                    if (!EqualityComparer<T>.Default.Equals((T)oldValue, newValue))
                    {
                        notifyFields[index] = newValue;
                        OnPropertyChanged?.Invoke(this, index, propertyName, newValue);
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual void Reset()
        {
            OnPropertyChanged = null;
            for (int i = 0; i < notifyFields.Length; i++)
            {
                notifyFields[i] = null;
            }
        }

        public void NotifyAll()
        {
            for (int i = 0; i < notifyFields.Length; i++)
            {
                if (notifyFields[i] != null)
                {
                    OnPropertyChanged?.Invoke(this, i, propertyNames[i], notifyFields[i]);
                }
            }
        }
    }
#endif
}
