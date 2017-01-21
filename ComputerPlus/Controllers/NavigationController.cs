﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage.Forms;
using ComputerPlus.Extensions.Gwen;

namespace ComputerPlus.Controllers
{
    class NavigationController
    {
        protected internal struct NavigationEntry
        {
            internal GwenForm form;
            internal bool isRoot;
            internal NavigationEntry(GwenForm form, bool isRoot = false)
            {
                this.form = form;
                this.isRoot = isRoot;
            }          
        }

        internal delegate void FormChangeEventArgs(object sender, NavigationEntry form);
        internal event FormChangeEventArgs OnFormAdded;
        internal event FormChangeEventArgs OnFormRemoved;

        Stack<NavigationEntry> Stack = new Stack<NavigationEntry>();
        //Stack<NavigationEntry> Stack
        //{
        //    get
        //    {
        //        //@TODO GWEN Forms have no closed event exposed... maybe there is a better solution.. right now we'll let gwen forms delete themselves on close
        //        var entries = mStack.Where(x => x.form != null && x.form.IsOpen());
        //        if (entries.Intersect(mStack).Count() > 1)
        //        {
        //            Function.Log("entries.Interset");
        //            mStack = new Stack<NavigationEntry>(entries);
        //        }
        //        return mStack;
        //    }
        //}

        internal int Size
        {
            get
            {
                return Stack.Count;
            }
        }

        internal GwenForm Head
        {
            get
            {
                return Stack.Count > 0 ? Stack.Peek().form : null;
            }
        }

        internal List<GwenForm> Tail
        {
            get
            {
                var stack = Stack;
                return stack.Count > 1 ?  Stack.Skip(1).Select(x => x.form).ToList() : null;
            }
        }

        internal bool IsEmpty
        {
            get { return Stack.Any(); }
        }

        internal bool HasOpenForms
        {
            get { return Stack.Any(x => x.form != null && x.form.IsOpen()); }
        }

        internal void Push(GwenForm form)
        {
            var entry = new NavigationEntry(form);
            Stack.Push(entry);
            if (OnFormAdded != null)
                OnFormAdded(this, entry);
        }

        internal bool RemoveEntry(NavigationEntry entry)
        {
            try
            {
                if (Stack.Contains(entry))
                {
                    Function.Log("RemoveEntry Stack.Contains... before " + Stack.Count.ToString());
                    Stack = new Stack<NavigationEntry>(Stack.Where(x => !x.Equals(entry)));
                    Function.Log("RemoveEntry Stack.Contains... after " + Stack.Count.ToString());
                    return true;
                }                
            }
            catch(Exception e)
            {
                Function.Log(e.ToString());
            }
            return false;
        }

        internal bool Pop()
        {
            if (IsEmpty) return false;
            NavigationEntry entry;
            do
            {
                entry = Stack.Pop();
            }
            while (!IsEmpty && (entry.form != null && !entry.form.IsOpen())); //Forms can close 
                   
            if (OnFormRemoved != null)
                OnFormRemoved(this, entry);

            return true;
        }

        //internal void PopToRoot()
        //{
        //    if (!IsEmpty)
        //    {
        //        var first = Stack.First().form;
        //        this.Replace(first);
        //    }
        //}
      
        internal void Replace(GwenForm form)
        {
            Clear();
            var entry = new NavigationEntry(form, true);
            Stack.Push(entry);
            if (OnFormAdded != null)
                OnFormAdded(this, entry);
        }

        internal void Clear()
        {
            if (OnFormRemoved != null)
            {
                foreach (var entry in Stack.Where(x => x.form != null))
                {
                    OnFormRemoved(this, entry);
                }
            }
            Stack.Clear();
        }
        
    }
}