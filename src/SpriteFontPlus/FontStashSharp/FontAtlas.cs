using System.Runtime.InteropServices;
using StbSharp;

namespace FontStashSharp
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe class FontAtlas
    {
        public int cnodes;
        public int height;
        public int nnodes;
        public FontAtlasNode* nodes;
        public int width;

        public FontAtlas(int w, int h, int nnodes)
        {
            width = w;
            height = h;
            nodes = (FontAtlasNode*) CRuntime.malloc((ulong) (sizeof(FontAtlasNode) * nnodes));
            CRuntime.memset(nodes, 0, (ulong) (sizeof(FontAtlasNode) * nnodes));
            nnodes = 0;
            cnodes = nnodes;
            nodes[0].x = 0;
            nodes[0].y = 0;
            nodes[0].width = (short) w;
            this.nnodes++;
        }

        public int fons__atlasInsertNode(int idx, int x, int y, int w)
        {
            if (nnodes + 1 > cnodes)
            {
                cnodes = cnodes == 0 ? 8 : cnodes * 2;
                nodes = (FontAtlasNode*) CRuntime.realloc(nodes, (ulong) (sizeof(FontAtlasNode) * cnodes));
                if (nodes == null)
                    return 0;
            }

            for (var i = nnodes; i > idx; i--) nodes[i] = nodes[i - 1];
            nodes[idx].x = (short) x;
            nodes[idx].y = (short) y;
            nodes[idx].width = (short) w;
            nnodes++;
            return 1;
        }

        public void fons__atlasRemoveNode(int idx)
        {
            if (nnodes == 0)
                return;
            for (var i = idx; i < nnodes - 1; i++) nodes[i] = nodes[i + 1];
            nnodes--;
        }

        public void fons__atlasExpand(int w, int h)
        {
            if (w > width)
                fons__atlasInsertNode(nnodes, width, 0, w - width);
            width = w;
            height = h;
        }

        public void fons__atlasReset(int w, int h)
        {
            width = w;
            height = h;
            nnodes = 0;
            nodes[0].x = 0;
            nodes[0].y = 0;
            nodes[0].width = (short) w;
            nnodes++;
        }

        public int fons__atlasAddSkylineLevel(int idx, int x, int y, int w, int h)
        {
            if (fons__atlasInsertNode(idx, x, y + h, w) == 0)
                return 0;
            for (var i = idx + 1; i < nnodes; i++)
                if (nodes[i].x < nodes[i - 1].x + nodes[i - 1].width)
                {
                    var shrink = nodes[i - 1].x + nodes[i - 1].width - nodes[i].x;
                    nodes[i].x += (short) shrink;
                    nodes[i].width -= (short) shrink;
                    if (nodes[i].width <= 0)
                    {
                        fons__atlasRemoveNode(i);
                        i--;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }

            for (var i = 0; i < nnodes - 1; i++)
                if (nodes[i].y == nodes[i + 1].y)
                {
                    nodes[i].width += nodes[i + 1].width;
                    fons__atlasRemoveNode(i + 1);
                    i--;
                }

            return 1;
        }

        public int fons__atlasRectFits(int i, int w, int h)
        {
            var x = (int) nodes[i].x;
            var y = (int) nodes[i].y;
            if (x + w > width)
                return -1;
            var spaceLeft = w;
            while (spaceLeft > 0)
            {
                if (i == nnodes)
                    return -1;
                y = FontSystem.fons__maxi(y, nodes[i].y);
                if (y + h > height)
                    return -1;
                spaceLeft -= nodes[i].width;
                ++i;
            }

            return y;
        }

        public int fons__atlasAddRect(int rw, int rh, int* rx, int* ry)
        {
            var besth = height;
            var bestw = width;
            var besti = -1;
            var bestx = -1;
            var besty = -1;
            for (var i = 0; i < nnodes; i++)
            {
                var y = fons__atlasRectFits(i, rw, rh);
                if (y != -1)
                    if (y + rh < besth || y + rh == besth && nodes[i].width < bestw)
                    {
                        besti = i;
                        bestw = nodes[i].width;
                        besth = y + rh;
                        bestx = nodes[i].x;
                        besty = y;
                    }
            }

            if (besti == -1)
                return 0;
            if (fons__atlasAddSkylineLevel(besti, bestx, besty, rw, rh) == 0)
                return 0;
            *rx = bestx;
            *ry = besty;
            return 1;
        }
    }
}