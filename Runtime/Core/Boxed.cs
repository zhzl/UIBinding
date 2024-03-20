namespace UIBinding
{
    /// <summary>
    /// 装箱对象，对值类型进行主动装箱，减少GC
    /// </summary>
    public class Boxed<T>
    {
        public T value;

        public Boxed(T value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
