using System;
using System.Threading.Tasks;
using MmImageLoading.Extensions;
using MmImageLoading.Work;
using UIKit;

namespace MmImageLoading.Targets
{
    public class UIButtonTarget: UIViewTarget<UIButton>
    {
        public UIButtonTarget(UIButton control) : base(control)
        {
        }

        public override void Set(IImageLoaderTask task, UIImage image, bool animated)
        {
            if (task == null || task.IsCancelled)
                return;

            var control = Control;
            if (control == null)
                return;
            control.SetImage(image, UIControlState.Normal);
        }

        public override void SetAsEmpty(IImageLoaderTask task)
        {
            if (task == null || task.IsCancelled)
                return;

            var control = Control;
            if (control == null)
                return;

            control.SetImage(null, UIControlState.Normal);
        }
    }
}

