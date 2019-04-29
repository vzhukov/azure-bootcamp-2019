#pragma once

#include <gflags/gflags.h>
#include <functional>
#include <iostream>
#include <fstream>
#include <random>
#include <memory>
#include <chrono>
#include <vector>
#include <string>
#include <utility>
#include <algorithm>
#include <iterator>
#include <map>

#include <inference_engine.hpp>

#include <ie_iextension.h>

#include <opencv2/opencv.hpp>

#include "common.hpp"
#include "OpenVinoAgeGenderRecognizer.hpp"
#include "OpenVinoEmotionsRecognizer.hpp"

using namespace InferenceEngine;
using namespace cv;
using namespace std;

struct FaceDetectorResult {
	Rect location;
	float confidence;
	string label;
	float age;
	bool isMale;
	string emotion;

	vector<Point> landmarks;

	string getAge() {
		stringstream ss;
		ss << setprecision(2) << fixed << age;
		return ss.str();
	}
};

class OpenVinoFaceDetector {
public:

	OpenVinoFaceDetector(InferencePlugin& plugin) {
		map<string, string> config;
		network = plugin.LoadNetwork(InitNetwork(), config);
	}

	CNNNetwork InitNetwork() {
		CNNNetReader netReader;
		netReader.ReadNetwork(modelXml);
		netReader.ReadWeights(modelBin);
		InputsDataMap inputInfo(netReader.getNetwork().getInputsInfo());
		InputInfo::Ptr& input = inputInfo.begin()->second;
		inputName = inputInfo.begin()->first;
		input->setPrecision(Precision::U8);
		OutputsDataMap outputInfo(netReader.getNetwork().getOutputsInfo());
		DataPtr& output = outputInfo.begin()->second;
		outputName = outputInfo.begin()->first;
		const SizeVector outputDims = output->getTensorDesc().getDims();
		maxProposalCount = outputDims[2];
		objectSize = outputDims[3];
		output->setPrecision(Precision::FP32);
		return netReader.getNetwork();
	}

	ExecutableNetwork network;
	int maxProposalCount;
	int objectSize;
	string inputName;
	string outputName;
	OpenVinoAgeGenderRecognizer ageGenderDetector;
	OpenVinoEmotionsRecognizer emotionsDetector;

	vector<FaceDetectorResult> GetResults(FrameData& data, const float * res) {
		vector<FaceDetectorResult> items;

		for (int i = 0; i < maxProposalCount; i++) {
			FaceDetectorResult item;
			float confidence = res[i * objectSize + 2];

			if (confidence < 0.3) {
				continue;
			}

			item.label = static_cast<int>(res[i * objectSize + 1]);
			item.confidence = confidence;
			item.location.x = res[i * objectSize + 3] * data.width;
			item.location.y = res[i * objectSize + 4] * data.height;
			item.location.width = res[i * objectSize + 5] * data.width
					- item.location.x;
			item.location.height = res[i * objectSize + 6] * data.height
					- item.location.y;

			auto itemRect = item.location & Rect(0, 0, data.width, data.height);
			Mat face = data.Image(itemRect);
			auto ageRes = ageGenderDetector.GetResult(face);
			auto emotion = emotionsDetector.GetResult(face);
			item.emotion = emotion;
			item.age = ageRes.age;
			item.isMale = ageRes.isMale;

			items.push_back(item);
		}

		return items;
	}

	void ProcessFrame(FrameData& data) {
		InferRequest::Ptr request = network.CreateInferRequestPtr();
		frameToBlob(data.Image, request, inputName);
		request->Infer();

		auto buffer = request->GetBlob(outputName)->buffer();

		const float *detections = buffer.as<float *>();
		auto items = GetResults(data, detections);
		for (auto& item : items) {

			auto color = item.isMale ? _maleColor : _femaleColor;

			rectangle(data.Image, item.location, color, 2);
			textRectTop(data.Image, item.location, item.getAge(), color,
					_colorWhite);
			textRectBottom(data.Image, item.location, item.emotion, color,
					_colorWhite);

			for (auto& lm : item.landmarks) {
				circle(data.Image, lm, 2, _colorTeal, -1);
			}
		}

	}

private:
	string modelBase = "./models/face-detection-adas-0001";
	string modelXml = modelBase + ".xml";
	string modelBin = modelBase + ".bin";
};
