using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.UI.Controls.BulbMenu.Anchors;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Resources;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.RichText;

namespace AgentMulder.ReSharper.Plugin.Highlighting
{
    [RegisterHighlighter("Container Registration", EffectType = EffectType.GUTTER_MARK, GutterMarkType = typeof(ContainerGutterMark), Layer = 2001)]
    public class ContainerGutterMark : IconGutterMarkType
    {
        public ContainerGutterMark()
            : base(AlteringFeatuThemedIcons.GeneratedMembers.Id)
        {
        }

        private ISolution GetCurrentSolution()
        {
            return Shell.Instance.GetComponent<SolutionsManager>().Solution;
        }

        public override IEnumerable<BulbMenuItem> GetBulbMenuItems(IHighlighter highlighter)
        {
            yield return new BulbMenuItem(new ExecutableItem(() =>
            {
                ISolution solution = GetCurrentSolution();

                var clickable = solution?.GetComponent<IDaemon>().GetHighlighting(highlighter) as IClickableGutterHighlighting;
                clickable?.OnClick();

            }), 
                (highlighter.TryGetTooltip(HighlighterTooltipKind.GutterMark) ?? (RichTextBlock)"The item has no text").Lines.Aggregate((a, b) => a.Append(" ", TextStyle.Default).Append(b)),
                IconId, BulbMenuAnchors.PermanentBackgroundItems, true);
        }
    }
}