#include "Header.h"
#include "Direction.h"
#include "opencv2/video/tracking.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/highgui/highgui.hpp"
#include <opencv2/opencv.hpp>
using namespace cv;

const int thresh = 50, N = 11;
const double PI = 3.1416;
const int MAX_COUNT = 500;
//direction
int width, height;
Mat gray, prevGray, frame, mask, pyr;
int result = -6;

Direction dirleft;
Direction dirright;
Direction dirup;
Direction dirdown;

VideoCapture cap;
TermCriteria termcrit(CV_TERMCRIT_ITER | CV_TERMCRIT_EPS, 20, 0.3);
Size winSize(10, 10);


bool endFlag = false;
bool stopColorsFlag = false;
bool stopDirectionFlag = false;

int area;
int globalCounter;
Scalar outputColor;

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

	if (pointy <= (height / 2)){
		if (pointx >= (oneThirdW) && pointx <= (2 * oneThirdW)){
			return true;
		}

	}
	return false;

}

bool isDown(int pointx, int pointy){
	int oneThirdW = width / 3;
	int oneThirdH = height / 3;
	if (pointy >= (height / 2)){
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

__declspec(dllexport) void openCam(){
	cap = VideoCapture(0);
}
__declspec(dllexport) void releaseCam(){
	cap.release();
}

__declspec(dllexport) int getDirection(){
	if (!cap.isOpened())
	{
		return -1;
	}
	result = -6;
	if (cap.isOpened()){

		initDirections();

		vector<Point2f> points[2];

		int sumx = 0, sumy = 0;
		if (getAndConvertToBW(prevGray, cap) == -1){

			result = -1;
		}
		else{
			width = frame.cols;
			height = frame.rows;
			if (getAndConvertToBW(gray, cap) == -1){

				result = -1;
			}
			else{
				createMask();
				goodFeaturesToTrack(prevGray, points[1], MAX_COUNT, 0.01, 0.1, mask);
				if (getAndConvertToBW(gray, cap) == -1){

					result = -4;
				}
				else{

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
			}
		}
	}
	return result;
}

static double angle(Point pt1, Point pt2, Point pt0)
{
	double dx1 = pt1.x - pt0.x;
	double dy1 = pt1.y - pt0.y;
	double dx2 = pt2.x - pt0.x;
	double dy2 = pt2.y - pt0.y;
	return (dx1*dx2 + dy1*dy2) / sqrt((dx1*dx1 + dy1*dy1)*(dx2*dx2 + dy2*dy2) + 1e-10);
}
static int checkRange(){
	std::vector<double> colors = { outputColor[2], outputColor[1], outputColor[0] };
	int max = 0;
	int min = 0;
	for (size_t i = 0; i < colors.size(); i++){
		if (colors[max] < colors[i]){
			max = i;
		}
		if (colors[min] > colors[i]){
			min = i;
		}
	}

	if ((colors[0] / colors[max]) > 0.80 && (/*max == 2 && */colors[1] / colors[max] > 0.8) && colors[2] / colors[max] > 0.8){
		if (max == 0){
			return 1;
		}
		else{
			return 0;
		}

	}
	switch (max){
	case 2:

		return 5; //blue
		break;
	case 1:
		if ((colors[0] / colors[1]) > 0.70){

			return 3;//yellow
		}
		else{
			return 4; //green
		}
		break;
	case 0:
		if ((colors[1] / colors[0]) < 0.48){

			return 1; //red

		}
		else if ((colors[1] / colors[0]) > 0.75){

			return 3;
		}
		else{

			return 2; //orange
		}
		break;
	}
	return -1;

}
static void findSquares(const Mat& image)
{
	vector<vector<Point> > squares;

	Mat pyr, timg, gray0(image.size(), CV_8U), gray;

	// down-scale and upscale the image to filter out the noise
	pyrDown(image, pyr, Size(image.cols / 2, image.rows / 2));
	pyrUp(pyr, timg, image.size());
	vector<vector<Point> > contours;

	// find squares in every color plane of the image
	for (int c = 0; c < 3; c++)
	{
		int ch[] = { c, 0 };
		mixChannels(&timg, 1, &gray0, 1, ch, 1);

		//// try several threshold levels
		for (int l = 0; l < N; l++)
		{
			//	// hack: use Canny instead of zero threshold level.
			//	// Canny helps to catch squares with gradient shading
			if (l == 0)
			{
				// apply Canny. Take the upper threshold from slider
				// and set the lower to 0 (which forces edges merging)
				Canny(gray0, gray, 0, thresh, 5);
				// dilate canny output to remove potential
				// holes between edge segments
				dilate(gray, gray, Mat(), Point(-1, -1));
			}
			else
			{
				// apply threshold if l!=0:
				//     tgray(x,y) = gray(x,y) < (l+1)*255/N ? 255 : 0
				gray = gray0 >= (l + 1) * 255 / N;
			}

			//	// find contours and store them all as a list
			findContours(gray, contours, CV_RETR_LIST, CV_CHAIN_APPROX_SIMPLE);

			vector<Point> approx;

			vector<Scalar> mean_colors;
			int counter = 0;

			//	// test each contour
			for (size_t i = 0; i < contours.size(); i++)
			{
				// approximate contour with accuracy proportional
				// to the contour perimeter
				approxPolyDP(Mat(contours[i]), approx, arcLength(Mat(contours[i]), true)*0.02, true);

				// square contours should have 4 vertices after approximation
				// relatively large area (to filter out noisy contours)
				// and be convex.
				// Note: absolute value of an area is used because
				// area may be positive or negative - in accordance with the
				// contour orientation
				if (approx.size() == 4 &&
					fabs(contourArea(Mat(approx))) > 1000 && fabs(contourArea(Mat(approx))) < area &&
					isContourConvex(Mat(approx)))
				{
					double maxCosine = 0;

					for (int j = 2; j < 5; j++)
					{
						// find the maximum cosine of the angle between joint edges
						double cosine = fabs(angle(approx[j % 4], approx[j - 2], approx[j - 1]));
						maxCosine = MAX(maxCosine, cosine);
					}

					// if cosines of all angles are small
					// (all angles are ~90 degree) then write quandrange
					// vertices to resultant sequence
					//if it is ok, add mean to mean value and increment counter
					if (maxCosine < 0.3){
						squares.push_back(approx);
						Mat mask_image(image.size(), CV_8U, Scalar(0));
						drawContours(mask_image, contours, i, Scalar(255), CV_FILLED);
						mean_colors.push_back(mean(image, mask_image));
						counter++;
					}
				}

			}

			Scalar final;
			if (counter != 0){
				for (size_t i = 0; i < mean_colors.size(); i++){
					final[0] += mean_colors.at(i)[0];
					final[1] += mean_colors.at(i)[1];
					final[2] += mean_colors.at(i)[2];
					final[3] += mean_colors.at(i)[3];

				}

				globalCounter++;
				outputColor[0] += final[0] / counter;
				outputColor[1] += final[1] / counter;
				outputColor[2] += final[2] / counter;
				outputColor[3] += final[3] / counter;

			}
		}
	}

}

__declspec(dllexport) int getColor()
{
	int color = -1;
	globalCounter = 0;
	outputColor = (0, 0, 0);
	if (!cap.isOpened())
	{
		return -1;
	}

	Mat image;
	for (int i = 0; i < 3; i++)
	{
		
		cap >> image;
		area = (image.cols*image.rows) / 10;
		findSquares(image);
	}

	if (globalCounter != 0){

		outputColor[0] = outputColor[0] / globalCounter;
		outputColor[1] = outputColor[1] / globalCounter;
		outputColor[2] = outputColor[2] / globalCounter;
		outputColor[3] = outputColor[3] / globalCounter;

		color = checkRange();

	}
	else{
		color = -1;
	}
	return color;



}
