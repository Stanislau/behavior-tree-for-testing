namespace BehaviorTreeForTests.Tests.Common
{
    public abstract class Proxy<T>
    {
        private T item;

        public T Item
        {
            get
            {
                if (item == null)
                {
                    item = Create();
                }

                return item;
            }
        }

        protected abstract T Create();

        public void Instantiate()
        {
            _ = Item;
        }
    }
}