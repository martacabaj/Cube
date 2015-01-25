#include "opencv2/video/tracking.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/highgui/highgui.hpp"
#include <opencv2/opencv.hpp>
#include "Direction.h"
#include "Header.h"
using namespace cv;
using namespace std;

int width, height;
const double PI = 3.1416;
const int MAX_COUNT = 500;
Mat gray, prevGray, frame, mask, pyr;
int result=-6;
bool endFlag = false;
Direction dirleft;
Direction dirright;
Direction dirup;
Direction dirdown;
VideoCapture cap;
TermCriteria termcrit(CV_TERMCRIT_ITER | CV_TERMCRIT_EPS, 20, 0.3);
Size winSize(10, 10);
inline static double square(int a)
{
	return a * a;
}

bool isLeft(int pointx, int pointy){
	int oneThirdW = width / 3;
	int oneThirdH = height / 3;
	if (pointx <= (oneThirdW)){
		if (pointy >= (oneThirdH) && pointy <= (oneThirdH * 2)){
			return true;
		}
	}
	return false;

}

bool isRight(int pointx, int pointy){
	int oneThirdW = width / 3;
	int oneThirdH = height / 3;
	if (pointx >= (2 * oneThirdW)){
		if (pointy >= (oneThirdH) && pointy <= (oneThirdH * 2)){
			return true;
		}
	}
	return false;

}

bool isUp(int pointx, int pointy){

	int  oneThirdW = width / 3;

	if (pointy <= (height/2)){
		if (pointx >= (oneThirdW) && pointx <= (2 * oneThirdW)){
			return true;
		}

	}
	return false;

}

bool isDown(int pointx, int pointy){
	int oneThirdW = width / 3;
	int oneThirdH = height / 3;
	if (pointy >= (2 * oneThirdH)){
		if (pointx >= (oneThirdW) && pointx <= (2 * oneThirdW)){
			return true;
		}

	}
	return false;

}
int  getAndConvertToBW(Mat& output, VideoCapture& cap){
	cap >> frame;
	if (frame.empty())
		return -1;
	flip(frame, frame, 1);
	cvtColor(frame, output, CV_BGR2GRAY);
	return 1;
}
void createMask(){
	pyrDown(frame, pyr, Size(frame.cols / 2, frame.rows / 2));
	pyrUp(pyr, frame, frame.size());
	absdiff(prevGray, gray, mask);

	threshold(mask, mask, 35, 255, CV_THRESH_BINARY&& CV_THRESH_OTSU);
	Mat kernel_ero = getStructuringElement(MORPH_RECT, Size(2, 2));
	erode(mask, mask, kernel_ero);
}
void initDirections(){
	dirleft = Direction();
	dirright = Direction();
	dirup = Direction();
	dirdown = Direction();
}
__declspec(dllexport) int getDirection(){
	return result;
}
__declspec(dllexport) void end(){
	endFlag = true;
}
__declspec(dllexport) void start()
{
	endFlag = false;
	cap = VideoCapture(0);
	if (!cap.isOpened())
	{
		result= -1;
	}
	for (;;)
	{
		if (endFlag){
			cap.release();
			break;
		}
		initDirections();

		vector<Point2f> points[2];

		int sumx = 0, sumy = 0;
		if (getAndConvertToBW(prevGray, cap) == -1){

			result = -2;
		}
		width = frame.cols;
		height = frame.rows;
		if (getAndConvertToBW(gray, cap) == -1){

			result = -3;
		}

		createMask();
		goodFeaturesToTrack(prevGray, points[1], MAX_COUNT, 0.01, 0.1, mask);
		if (getAndConvertToBW(gray, cap) == -1){

			result = -4;
		}


		vector<uchar> status;
		vector<float> err;
		int hor = 0, ver = 0;

		if (!points[1].empty())
		{
			calcOpticalFlowPyrLK(prevGray, gray, points[1], points[0], status, err, winSize,
				3, termcrit, 0);
			int totalSum = 0;
			for (size_t i = 0; i < points[1].size(); i++)
			{

				if (!status[i])
					continue;

				int line_thickness; line_thickness = 1;

				CvScalar line_color; line_color = CV_RGB(255, 0, 0);

				CvPoint p, q;

				p.x = (int)points[1][i].x;
				p.y = (int)points[1][i].y;
				q.x = (int)points[0][i].x;
				q.y = (int)points[0][i].y;

				double hypotenuse; hypotenuse = sqrt(square(p.y - q.y) + square(p.x - q.x));
				int deltax = abs(p.x - q.x);
				int deltay = abs(p.y - q.y);
				if (deltax < 10 || deltay < 10){
					if (hypotenuse > 15){

						sumx += p.x;
						sumy += p.y;

						totalSum++;

						if (deltax < 10){

							if (p.y < q.y){
								dirdown.numOfPoints++;
							}
							else{
								dirup.numOfPoints++;
							}
						}
						else{

							if (p.x < q.x){
								dirright.numOfPoints++;
							}
							else{
								dirleft.numOfPoints++;
							}
						}
					}
				}
			}
			if (totalSum != 0){
				sumx /= totalSum;
				sumy /= totalSum;
			}
		}

		vector<int> dir = { dirleft.numOfPoints, dirup.numOfPoints, dirright.numOfPoints, dirdown.numOfPoints };
		int max = 0;
		int second = 0;
		for (size_t i = 0; i < dir.size(); i++){
			if (dir[max] < dir[i]){
				second = max;
				max = i;
			}

		}
		result = -7;
		if (dir[max] > 1){

			switch (max){
			case 0:
				if (isLeft(sumx, sumy)){

					result = 0;
				}
				break;
			case 1:

				if (isUp(sumx, sumy)){

					result = 1;
				}
				break;
			case 2:

				if (isRight(sumx, sumy)){

					result = 2;
				}
				break;
			case 3:

				if (isDown(sumx, sumy)){

					result = 3;
				}
				break;

			}
		}
	}
	result = -5;

}
