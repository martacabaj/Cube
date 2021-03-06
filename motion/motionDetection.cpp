#include "stdwx.h"
#include "opencv2/video/tracking.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/highgui/highgui.hpp"
#include <iostream>
#include <ctype.h>
#include<opencv2/opencv.hpp>
#include "Direction.h"
using namespace cv;
using namespace std;

const double PI = 3.1416;
inline static double square(int a)
{
	return a * a;
}
int main(int argc, char** argv)
{
	VideoCapture cap = VideoCapture(0);
	int width, height;
	TermCriteria termcrit(CV_TERMCRIT_ITER | CV_TERMCRIT_EPS, 20, 0.3);
	Size winSize(10, 10);
	if (!cap.isOpened())
	{
		cout << "Could not initialize capturing...\n";
		return 0;
	}
	const int MAX_COUNT = 500;
	BackgroundSubtractorMOG2 bg;
		bg.set("nmixtures", 3);
	    bg.set("detectShadows", false);
	
		int counter = 0;
		vector<int> dir;
		for (;;)
	{
			Direction dirleft = Direction();
			Direction dirright = Direction();
			Direction dirup = Direction();
			Direction dirdown = Direction();
			int right=0, left=0, up=0, down=0;
			for (int i = 0; i < 3; i++){
				counter++;
				//cout << counter<<"\n";
				Mat gray, prevGray, frame, mask, pyr;
				cap >> frame;
				width = frame.cols;
				height = frame.rows;
				if (frame.empty())
					break;
				flip(frame, frame, 1);
				cvtColor(frame, prevGray, CV_BGR2GRAY);
				vector<Point2f> points[2];

				//cornerSubPix(prevGray, points[1], winSize, Size(-1, -1), termcrit);
				cap >> frame;
				if (frame.empty())
					break;
				flip(frame, frame, 1);
				cvtColor(frame, gray, CV_BGR2GRAY);
				pyrDown(frame, pyr, Size(frame.cols / 2, frame.rows / 2));
				pyrUp(pyr, frame, frame.size());
				absdiff(prevGray, gray, mask);
				//bg.operator ()(frame, mask);
				threshold(mask, mask, 35, 255, CV_THRESH_BINARY&& CV_THRESH_OTSU);
				Mat kernel_ero = getStructuringElement(MORPH_RECT, Size(2, 2));
				erode(mask, mask, kernel_ero);

				imshow("mask", mask);

				goodFeaturesToTrack(prevGray, points[1], MAX_COUNT, 0.01, 0.1, mask);

				cap >> frame;

				if (frame.empty())
					break;
				flip(frame, frame, 1);
				cvtColor(frame, gray, CV_BGR2GRAY);

				vector<uchar> status;
				vector<float> err;
				int hor = 0, ver = 0;
				if (!points[1].empty())
				{
					calcOpticalFlowPyrLK(prevGray, gray, points[1], points[0], status, err, winSize,
						3, termcrit, 0);
					size_t i;

					for (i = 0; i < points[1].size(); i++)
					{

						if (!status[i])
							continue;

						circle(frame, points[1][i], 3, Scalar(0, 255, 0), -1, 8);

						int line_thickness; line_thickness = 1;

						CvScalar line_color; line_color = CV_RGB(255, 0, 0);
						/* Let's make the flow field look nice with arrows. */
						/* The arrows will be a bit too short for a nice visualization because of the
						high framerate
						* (ie: there's not much motion between the frames). So let's lengthen them
						by a factor of 3.
						*/
						CvPoint p, q;
						p.x = (int)points[1][i].x;
						p.y = (int)points[1][i].y;
						q.x = (int)points[0][i].x;
						q.y = (int)points[0][i].y;
						double angle; angle = atan2((double)p.y - q.y, (double)p.x - q.x);

						double hypotenuse; hypotenuse = sqrt(square(p.y - q.y) + square(p.x - q.x))
							;

						/* Here we lengthen the arrow by a factor of three. */
						//q.x = (int)(p.x - 3 * hypotenuse * cos(angle));
						//q.y = (int)(p.y - 3 * hypotenuse * sin(angle));
						//cout << angle*180/PI << "\n";
						int deltax = abs(p.x - q.x);
						int deltay = abs(p.y - q.y);
						if (deltax < 10 || deltay < 10){
							if (hypotenuse > 15){
								line(frame, p, q, line_color, line_thickness, CV_AA, 0);

								p.x = (int)(q.x + 9 * cos(angle + PI / 4));
								p.y = (int)(q.y + 9 * sin(angle + PI / 4));
								line(frame, p, q, line_color, line_thickness, CV_AA, 0);
								p.x = (int)(q.x + 9 * cos(angle - PI / 4));
								p.y = (int)(q.y + 9 * sin(angle - PI / 4));
								line(frame, p, q, line_color, line_thickness, CV_AA, 0);
								if (deltax < 10){
									ver++;
									if (p.y < q.y){
										dirdown.numOfPoints++;
										if (dirdown.numOfPoints == 1){
											dirdown.maxPoint = q.y;
											dirdown.minPoint = p.y;
										}
										dirdown.maxPoint = dirdown.maxPoint < q.y ? q.y : dirdown.maxPoint;
										dirdown.minPoint = dirdown.minPoint > p.y ? p.y : dirdown.minPoint;
										//down++;
									}
									else{
										dirup.numOfPoints++;
										if (dirup.numOfPoints == 1){
											dirup.maxPoint = q.y;
											dirup.minPoint = p.y;
										}
										dirup.maxPoint = dirup.maxPoint < q.y ? q.y : dirup.maxPoint;
										dirup.minPoint = dirup.minPoint > p.y ? p.y : dirup.minPoint;
										//up++;
									}
								}
								else{
									hor++;
									if (p.x < q.x){
										dirright.numOfPoints++;
										if (dirright.numOfPoints == 1){
											dirright.maxPoint = q.x;
											dirright.minPoint = p.x;
										}
										dirright.maxPoint = dirright.maxPoint < q.x ? q.x : dirright.maxPoint;
										dirright.minPoint = dirright.minPoint > p.x ? p.x : dirright.minPoint;
										//right++;
									}
									else{
										dirleft.numOfPoints++;
										if (dirleft.numOfPoints == 1){
											dirleft.maxPoint = q.x;
											dirleft.minPoint = p.x;
										}
										dirleft.maxPoint = dirleft.maxPoint < q.x ? q.x : dirleft.maxPoint;
										dirleft.minPoint = dirleft.minPoint > p.x ? p.x : dirleft.minPoint;
										//left++;
									}
								}
							}
						}
					}

				}
				
				imshow("lk", frame);
			}
		//cout << "hor " << hor << " ver " << ver << "\n" ;
	//	cout << left<<" " << up<<" "<<right << " " << down << "\n";
			vector<int> dir = { dirleft.numOfPoints, dirup.numOfPoints, dirright.numOfPoints, dirdown.numOfPoints };
		int max = 0;
		int second = 0;
		for (size_t i = 0; i < dir.size(); i++){
			if (dir[max] < dir[i]){
				second = max;
				max = i;
			}
			
		}
		if (dir[max] != 0 ){
			
			switch (max){
			case 0:
				
				cout << width << " " << abs(dirleft.maxPoint - dirleft.minPoint) << "\n";
				if (abs(dirleft.maxPoint - dirleft.minPoint) >= 0.6*width){
					cout << "left" << "\n";
				}
				break;
			case 1:
				
				cout << height<<" "<<abs(dirup.maxPoint - dirup.minPoint) << "\n";
				if (abs(dirup.maxPoint - dirup.minPoint) >= 0.6*height){
					cout << "up" << "\n";
				}
				break;
			case 2:
			
				cout << width << " r " << abs(dirright.maxPoint - dirright.minPoint) << "\n";
				if (abs(dirright.maxPoint - dirright.minPoint) >= 0.6*width){
					cout << "right" << "\n";
				}
				break;
			case 3:
				
				cout << height << " " << abs(dirdown.maxPoint - dirdown.minPoint) << "\n";
				if (abs(dirdown.maxPoint - dirdown.minPoint) >= 0.6*height){
					cout << "down" << "\n";
				}
				break;

			}
		}
		else{
			cout << "nothing" << "\n";
		}
		
		char c = (char)waitKey(10);
		if (c == 27)
			break;
		
	}

	return 0;
}
