using System.IO;
using System.Drawing;
using System.Net;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace CarShopLibrary
{
    internal class OpenXMLImageHelper
    {
        internal static Stream FromImageUrlToStream(string imgUrl)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(imgUrl);
            req.UseDefaultCredentials = true;
            req.PreAuthenticate = true;
            req.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            return resp.GetResponseStream();
        }

        internal static Stream FromImageObjToStream(Image imgObj)
        {
            MemoryStream stream = new MemoryStream();
            imgObj.Save(stream, imgObj.RawFormat);
            stream.Position = 0;
            return stream;
        }

        // To insert the picture
        internal static Drawing DrawingManager(string relationshipId, string name, Int64Value cxVal, Int64Value cyVal, string impPosition)
        {
            string haPosition = impPosition;
            if (string.IsNullOrEmpty(haPosition))
            {
                haPosition = "left";
            }
            // Define the reference of the image.
            DW.Anchor anchor = new DW.Anchor();
            anchor.Append(new DW.SimplePosition() { X = 0L, Y = 0L });
            anchor.Append(
                new DW.HorizontalPosition(
                    new DW.HorizontalAlignment(haPosition)
                )
                {
                    RelativeFrom =
                      DW.HorizontalRelativePositionValues.Margin
                }
            );
            anchor.Append(
                new DW.VerticalPosition(
                    new DW.PositionOffset("0")
                )
                {
                    RelativeFrom =
                    DW.VerticalRelativePositionValues.Paragraph
                }
            );
            anchor.Append(
                new DW.Extent()
                {
                    Cx = cxVal,
                    Cy = cyVal
                }
            );
            anchor.Append(
                new DW.EffectExtent()
                {
                    LeftEdge = 0L,
                    TopEdge = 0L,
                    RightEdge = 0L,
                    BottomEdge = 0L
                }
            );
            if (!string.IsNullOrEmpty(impPosition))
            {
                anchor.Append(new DW.WrapSquare() { WrapText = DW.WrapTextValues.BothSides });
            }
            else
            {
                anchor.Append(new DW.WrapTopBottom());
            }
            anchor.Append(
                new DW.DocProperties()
                {
                    Id = (UInt32Value)1U,
                    Name = name
                }
            );
            anchor.Append(
                new DW.NonVisualGraphicFrameDrawingProperties(
                      new A.GraphicFrameLocks() { NoChangeAspect = true })
            );
            anchor.Append(
                new A.Graphic(
                      new A.GraphicData(
                        new PIC.Picture(

                          new PIC.NonVisualPictureProperties(
                            new PIC.NonVisualDrawingProperties()
                            {
                                Id = (UInt32Value)0U,
                                Name = name + ".jpg"
                            },
                            new PIC.NonVisualPictureDrawingProperties()),

                            new PIC.BlipFill(
                                new A.Blip(
                                    new A.BlipExtensionList(
                                        new A.BlipExtension()
                                        {
                                            Uri =
                                            "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                        })
                                )
                                {
                                    Embed = relationshipId,
                                    CompressionState =
                                    A.BlipCompressionValues.Print
                                },
                                new A.Stretch(
                                    new A.FillRectangle())),

                          new PIC.ShapeProperties(

                            new A.Transform2D(
                              new A.Offset() { X = 0L, Y = 0L },

                              new A.Extents()
                              {
                                  Cx = cxVal,
                                  Cy = cyVal
                              }),

                            new A.PresetGeometry(
                              new A.AdjustValueList()
                            )
                            { Preset = A.ShapeTypeValues.Rectangle }
                          )
                        )
                  )
                      { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
            );

            anchor.DistanceFromTop = (UInt32Value)0U;
            anchor.DistanceFromBottom = (UInt32Value)0U;
            anchor.DistanceFromLeft = (UInt32Value)114300U;
            anchor.DistanceFromRight = (UInt32Value)114300U;
            anchor.SimplePos = false;
            anchor.RelativeHeight = (UInt32Value)251658240U;
            anchor.BehindDoc = true;
            anchor.Locked = false;
            anchor.LayoutInCell = true;
            anchor.AllowOverlap = true;

            Drawing element = new Drawing();
            element.Append(anchor);

            return element;
        }
    }
}
