/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 3/17/2025, 11:56:27 AM
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using System.Collections;

namespace Utils.Container
{

    public class FixedQueue<T>(int capacity) : IEnumerable<T>
    {

        private readonly Queue<T> _queue = new();
        private readonly int _capacity = capacity;

        public void Enqueue(T item)
        {
            if (_queue.Count >= _capacity)
            {
                _queue.Dequeue();
            }
            _queue.Enqueue(item);
        }

        public T Dequeue()
        {
            return _queue.Dequeue();
        }

        public T Peek()
        {
            return _queue.Peek();
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public int Count()
        {
            return _queue.Count;
        }

        public bool Contains(T item)
        {
            return _queue.Contains(item);
        }

        public T[] ToArray()
        {
            return [.. _queue];
        }

        public void CopyTo(T[] array, int index)
        {
            _queue.CopyTo(array, index);
        }

        public FixedQueue<T> Clone()
        {
            var clone = new FixedQueue<T>(_capacity);
            foreach (var item in _queue)
            {
                clone.Enqueue(item);
            }
            return clone;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

    }
}