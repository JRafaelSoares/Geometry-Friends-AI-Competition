namespace GeometryFriendsAgents
{
    using System;
    using System.Collections.Generic;

    internal class TreeNode<T>
    {
        private int index;
        public float weight;
        public T content;
        public TreeNode<T> parent;
        public List<TreeNode<T>> children;

        public TreeNode(TreeNode<T> t)
        {
            this.index = t.getIndex();
            this.weight = t.weight;
            this.content = t.content;
            this.parent = null;
            this.children = new List<TreeNode<T>>();
        }

        public TreeNode(T in_content, int in_idx)
        {
            this.content = in_content;
            this.index = in_idx;
            this.weight = 0f;
            this.parent = null;
            this.children = new List<TreeNode<T>>();
        }

        public int AddChild(TreeNode<T> child)
        {
            this.children.Add(child);
            child.parent = (TreeNode<T>) this;
            return 0;
        }

        public bool checkRoot(TreeNode<T> root)
        {
            for (TreeNode<T> node = (TreeNode<T>) this; node != null; node = node.parent)
            {
                if (ReferenceEquals(node, root))
                {
                    return true;
                }
            }
            return false;
        }

        public int getIndex() => 
            this.index;

        public float getweightuntiltop()
        {
            float num = 0f;
            for (TreeNode<T> node = (TreeNode<T>) this; node != null; node = node.parent)
            {
                num += this.weight;
            }
            return num;
        }

        public bool isparent(TreeNode<T> in_tn) => 
            ReferenceEquals(this.parent, in_tn);

        public void print()
        {
            foreach (TreeNode<T> node in this.children)
            {
                node.print();
            }
        }

        public void RemoveChild(TreeNode<T> child)
        {
            this.children.Remove(child);
            child.parent = null;
        }

        public TreeNode<T> search(int index)
        {
            Queue<TreeNode<T>> queue = new Queue<TreeNode<T>>();
            queue.Enqueue((TreeNode<T>) this);
            int num = Parameter.LOOP_BREAK;
            while ((queue.Count > 0) && (--num > 0))
            {
                TreeNode<T> node = queue.Dequeue();
                if (node.getIndex() == index)
                {
                    return node;
                }
                foreach (TreeNode<T> node2 in node.children)
                {
                    queue.Enqueue(node2);
                }
            }
            return null;
        }

        public void setIndex(int in_idx)
        {
            this.index = in_idx;
        }
    }
}

