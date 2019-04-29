#pragma once

#include <gflags/gflags.h>
#include <string>
#include <utility>

#include <inference_engine.hpp>

#include <ie_iextension.h>

#include <opencv2/opencv.hpp>

using namespace InferenceEngine;
using namespace cv;
using namespace std;


Scalar _maleColor = Scalar(212, 120, 0);
Scalar _femaleColor = Scalar(140, 0, 227);

// black text
Scalar _colorYellow = Scalar(0, 185, 255);
Scalar _colorOrangeLighter = Scalar(0, 140, 255);

// white text
Scalar _colorMagenta = Scalar(158, 0, 180);
Scalar _colorPurple = Scalar(145, 45, 92);
Scalar _colorBlue = Scalar(212, 120, 0);
Scalar _colorTeal = Scalar(114, 130, 0);
Scalar _colorGreen = Scalar(16, 124, 16);
Scalar _colorRed = Scalar(35, 17, 232);

Scalar _colorWhite = Scalar(255, 255, 255);
Scalar _colorBlack = Scalar(0, 0, 0);

int _fontFace = cv::FONT_HERSHEY_SIMPLEX;


// From Intel documentation
void matU8ToBlob(Mat& img, Blob::Ptr& blob, int index = 0) {
	SizeVector blobSize = blob->getTensorDesc().getDims();
	const int channels = blobSize[1];
	const int height = blobSize[2];
	const int width = blobSize[3];

	uint8_t* blob_data = blob->buffer().as<uint8_t*>();
	Mat new_img(img);
	auto imgSize = img.size();
	if (width != imgSize.width || height != imgSize.height) {
		resize(img, new_img, Size(width, height));
		if(width==60){
			imshow("face", new_img);
		}
	}

	int offset = index * width * height * channels;

	for (int c = 0; c < channels; c++) {
		for (int h = 0; h < height; h++) {
			for (int w = 0; w < width; w++) {
				blob_data[offset + c * width * height + h * width + w] =
						new_img.at<Vec3b>(h, w)[c];
			}
		}
	}
}

void frameToBlob(Mat& frame, InferRequest::Ptr& inferRequest,
		const string& inputName) {
	Blob::Ptr frameBlob = inferRequest->GetBlob(inputName);
	matU8ToBlob(frame, frameBlob);
}

void textRectTop(Mat& frame, Rect& rect, string text, Scalar bgColor,
		Scalar txtColor) {
	Size size = getTextSize(text, _fontFace, 0.4, 1, 0);
	auto bgPointTopLeft = Point(rect.x, rect.y);
	auto bgPointBottomRight = Point(rect.x + size.width * 1.2, rect.y + size.height);
	auto txtPoint = Point(rect.x, rect.y + size.height);

	rectangle(frame, bgPointTopLeft, bgPointBottomRight, bgColor, -1);
	putText(frame, text, txtPoint, _fontFace, 0.4, txtColor, 1, 8);
}

void textRectBottom(Mat& frame, Rect& rect, string text, Scalar bgColor,
		Scalar txtColor) {
	Size size = getTextSize(text, _fontFace, 0.4, 1, 0);
	auto bgPointTopLeft = Point(rect.x - 1, rect.y + rect.height);
	auto bgPointBottomRight = Point(rect.x + size.width * 1.2,
			rect.y + rect.height + size.height * 1.2);
	auto txtPoint = Point(rect.x, rect.y + rect.height + size.height);

	rectangle(frame, bgPointTopLeft, bgPointBottomRight, bgColor, -1);
	putText(frame, text, txtPoint, _fontFace, 0.4, txtColor, 1, 8);
}

