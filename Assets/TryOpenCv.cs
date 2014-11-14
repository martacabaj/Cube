using UnityEngine;
using System.Collections;
using OpenCvSharp;
using System;

public class TryOpenCv : MonoBehaviour
{

		public GameObject planeObj;
		public WebCamTexture webcamTexture;
		public Texture2D texImage;
		public string deviceName;
		private int devId = 1;
		private int imWidth = 640;
		private int imHeight = 480;
		private string errorMsg = "No errors found!";
		static IplImage matrix;
	
		// Use this for initialization
		void Start ()
		{
				WebCamDevice[] devices = WebCamTexture.devices;
				Debug.Log ("num:" + devices.Length);
		
				for (int i=0; i<devices.Length; i++) {
						print (devices [i].name);
						if (devices [i].name.CompareTo (deviceName) == 1) {
								devId = i;
						}
				}
		
				if (devId >= 0) {
						planeObj = GameObject.Find ("Plane");
						texImage = new Texture2D (imWidth, imHeight, TextureFormat.RGB24, false);
						webcamTexture = new WebCamTexture (devices [devId].name, imWidth, imHeight, 60);
						webcamTexture.Play ();
			
						matrix = new IplImage (imWidth, imHeight, BitDepth.U8, 3);
				}
		}
	
		void Update ()
		{
				if (devId >= 0) {

						Texture2DtoIplImage (matrix);

						if (webcamTexture.didUpdateThisFrame) {
								
								DrawSquares (matrix, ConvertIntoGreyScale (matrix));
						}
			
			
				} else {
						Debug.Log ("Can't find camera!");
				}
		}
	
		void OnGUI ()
		{
				GUI.Label (new Rect (200, 200, 100, 90), errorMsg);
		}
	
		void IplImageToTexture2D (IplImage image)
		{
				int jBackwards = imHeight;
		
				for (int i = 0; i < imHeight; i++) {
						for (int j = 0; j < imWidth; j++) {
								float b = (float)image [i, j].Val0;
								float g = (float)image [i, j].Val1;
								float r = (float)image [i, j].Val2;
								Color color = new Color (r / 255.0f, g / 255.0f, b / 255.0f);
				
				
								jBackwards = imHeight - i - 1; // notice it is jBackward and i
								texImage.SetPixel (j, jBackwards, color);
						}
				}
				texImage.Apply ();
				planeObj.renderer.material.mainTexture = texImage;
		
		}
	
		void Texture2DtoIplImage (IplImage image)
		{
				int jBackwards = imHeight;
		
				for (int v=0; v<imHeight; ++v) {
						for (int u=0; u<imWidth; ++u) {
				
								CvScalar col = new CvScalar ();
								col.Val0 = (double)webcamTexture.GetPixel (u, v).b * 255;
								col.Val1 = (double)webcamTexture.GetPixel (u, v).g * 255;
								col.Val2 = (double)webcamTexture.GetPixel (u, v).r * 255;
				
								jBackwards = imHeight - v - 1;
				
								image.Set2D (jBackwards, u, col);
								//matrix [jBackwards, u] = col;
						}
				}
			
		}

		static double Angle (CvPoint pt1, CvPoint pt2, CvPoint pt0)
		{
				double dx1 = pt1.X - pt0.X;
				double dy1 = pt1.Y - pt0.Y;
				double dx2 = pt2.X - pt0.X;
				double dy2 = pt2.Y - pt0.Y;
				return (dx1 * dx2 + dy1 * dy2) / Math.Sqrt ((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2) + 1e-10);
		}
		private double cosineOfAngle (CvPoint pt0, CvPoint pt1, CvPoint pt2)
		{
				double a = LengthOfSegment (pt0, pt1);
				double b = LengthOfSegment (pt1, pt2);
				double c = LengthOfSegment (pt0, pt2);

				double cosine = (a * a + b * b - c * c) / (2 * a * b);
				return cosine;
		}
		private double LengthOfSegment (CvPoint pt0, CvPoint pt1)
		{
				double dx1 = pt1.X - pt0.X;
				double dy1 = pt1.Y - pt0.Y;
				return Math.Sqrt (Math.Pow (dx1, 2) + Math.Pow (dy1, 2));
		}
		private CvPoint[] ConvertIntoGreyScale (IplImage image)
		{
				IplImage imageCopy = image.Clone ();
				IplImage imgGreyScale = new IplImage (imWidth, imHeight, BitDepth.U8, 1);
				IplImage pyrImage = new IplImage (image.Width / 2, image.Height / 2, BitDepth.U8, 3);
				using (CvMemStorage storage = new CvMemStorage()) {
						CvSeq<CvPoint> squares = new CvSeq<CvPoint> (SeqType.Zero, CvSeq.SizeOf, storage);
						Cv.PyrDown (imageCopy, pyrImage, CvFilter.Gaussian5x5);
						Cv.PyrUp (pyrImage, imageCopy, CvFilter.Gaussian5x5);
						IplImage tgray = new IplImage (imWidth, imHeight, BitDepth.U8, 1);
						
						for (int c = 0; c < 3; c++) {
								// extract the c-th color plane
								imageCopy.COI = c + 1;
								Cv.Copy (imageCopy, tgray, null);
				
								// try several threshold levels
								int N = 11;
								for (int l = 0; l < N; l++) {
										// hack: use Canny instead of zero threshold level.
										// Canny helps to catch squares with gradient shading   
										if (l == 0) {
												// apply Canny. Take the upper threshold from slider
												// and set the lower to 0 (which forces edges merging) 
												Cv.Canny (tgray, imgGreyScale, 0, 50, ApertureSize.Size5);
												// dilate canny output to remove potential
												// holes between edge segments 
												Cv.Dilate (imgGreyScale, imgGreyScale, null, 1);
										} else {
												// apply threshold if l!=0:
												//     tgray(x,y) = gray(x,y) < (l+1)*255/N ? 255 : 0
												Cv.Threshold (tgray, imgGreyScale, (l + 1) * 255.0 / N, 255, ThresholdType.Binary);
										}
					
										// find contours and store them all as a list
										CvSeq<CvPoint> contours;
										Cv.FindContours (imgGreyScale, storage, out contours, CvContour.SizeOf, ContourRetrieval.List, ContourChain.ApproxSimple, new CvPoint (0, 0));
					
										// test each contour
										while (contours != null) {
												// approximate contour with accuracy proportional
												// to the contour perimeter
												CvSeq<CvPoint> result = Cv.ApproxPoly (contours, CvContour.SizeOf, storage, ApproxPolyMethod.DP, contours.ContourPerimeter () * 0.02, false);
												// square contours should have 4 vertices after approximation
												// relatively large area (to filter out noisy contours)
												// and be convex.
												// Note: absolute value of an area is used because
												// area may be positive or negative - in accordance with the
												// contour orientation
					
												if (result.Total == 4 && Math.Abs (result.ContourArea (CvSlice.WholeSeq)) > 100 && Math.Abs (result.ContourArea (CvSlice.WholeSeq)) < (image.Height * image.Width) / 4 && result.CheckContourConvexity ()) {
														bool isRect = true;

														isRect = Math.Abs (cosineOfAngle (result [0].Value, result [1].Value, result [2].Value)) <= 0.1 ? true : false;
														isRect = Math.Abs (cosineOfAngle (result [1].Value, result [2].Value, result [3].Value)) <= 0.1 ? true : false;
														isRect = Math.Abs (cosineOfAngle (result [2].Value, result [3].Value, result [0].Value)) <= 0.1 ? true : false;
														isRect = Math.Abs (cosineOfAngle (result [3].Value, result [0].Value, result [1].Value)) <= 0.1 ? true : false;
													
//													
														if (isRect) {
													
						
																int maxX = int.MinValue, minX = int.MaxValue, maxY = int.MinValue, minY = int.MaxValue;
																CvSeq<CvPoint> pointInside = new CvSeq<CvPoint> (SeqType.Zero, CvSeq.SizeOf, storage);
																for (int i = 0; i < 4; i++) {

																		squares.Push (result [i].Value);

																		maxX = result [i].Value.X > maxX ? result [i].Value.X : maxX;
																		maxY = result [i].Value.Y > maxY ? result [i].Value.Y : maxY;

																		minX = result [i].Value.X < minX ? result [i].Value.X : minX;

																		minY = result [i].Value.Y < minY ? result [i].Value.Y : minY;
								
																		//IplImage hsv = new IplImage (maxX - minX, maxY - minY, BitDepth.U8, 3);
																		for (int x=minX; x<=maxX; x++) {
																				for (int y=minY; y<=maxY; y++) {
																						
																		
																						double testResult = Cv.PointPolygonTest (result, new CvPoint2D32f (x, y), false);
																																
																						if (testResult >= 0) {
																								//hsv [x - minX] [y - minY] = image [x] [y];
//																								Debug.Log ("push");
//																								//pointInside.Push (image [x] [y]);
//								
																						}

																				}
																		}
																		
																		//CV.DrawContours(image, contours, idx, Scalar(255), CV_FILLED);
																		//squares.Push (result [i].Value);

																}
														}
												}
						
												// take the next contour
												contours = contours.HNext;
										}
								}
						}
			
						// release all the temporary images
						imgGreyScale.Dispose ();
						pyrImage.Dispose ();
						tgray.Dispose ();
						imageCopy.Dispose ();
			
						return squares.ToArray ();

				}
		
		
		
		
		}
		private void DrawSquares (IplImage img, CvPoint[] squares)
		{
				
				// read 4 sequence elements at a time (all vertices of a square)
				for (int i = 0; i < squares.Length; i += 4) {
						CvPoint[] pt = new CvPoint[4];
				
						// read 4 vertices
						pt [0] = squares [i + 0];
						pt [1] = squares [i + 1];
						pt [2] = squares [i + 2];
						pt [3] = squares [i + 3];

						// draw the square as a closed polyline 
		
						Cv.FillPoly (img, new CvPoint[][] { pt }, /*true,*/CvColor.Red,/* 3,*/LineType.AntiAlias, 0);
						
			
						// show the resultant image
						IplImageToTexture2D (img);
				}
		}
		private void colorDetection (IplImage image)
		{
				image.CvtColor (image, ColorConversion.BgrToHsv);
				//CvArr arr = CvScalar ;
				//image.InRange (new CvScalar (170, 150, 60), new CvScalar (179, 255, 255), image);
				IplImageToTexture2D (image);
		}
}
