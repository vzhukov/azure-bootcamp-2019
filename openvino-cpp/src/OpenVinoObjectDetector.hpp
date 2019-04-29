#pragma once

#include <gflags/gflags.h>
#include <functional>
#include <iostream>
#include <chrono>
#include <string>
#include <utility>
#include <algorithm>
#include <vector>
#include <iterator>
#include <inference_engine.hpp>
#include <ie_iextension.h>
#include <opencv2/opencv.hpp>
#include "common.hpp"

using namespace InferenceEngine;
using namespace cv;
using namespace std;

struct ObjectResult {
	float label;
	float confidence;
	Rect location;
};

class OpenVinoObjectDetector {
public:

	OpenVinoObjectDetector(InferencePlugin& plugin) {
		network = plugin.LoadNetwork(InitNetwork(), {});
	}

	CNNNetwork InitNetwork() {
		CNNNetReader netReader;
		netReader.ReadNetwork(modelXml);
		netReader.getNetwork().setBatchSize(1);
		netReader.ReadWeights(modelBin);
		auto net = netReader.getNetwork();

		net.setBatchSize(1);

		InferenceEngine::InputsDataMap input_info(net.getInputsInfo());
		InferenceEngine::SizeVector inputDims;
		for (auto &item : input_info) {
			auto input_data = item.second;
			inputName = item.first;
			input_data->setPrecision(Precision::U8);
			input_data->setLayout(Layout::NCHW);
			inputDims = input_data->getDims();
		}


		InferenceEngine::OutputsDataMap output_info(net.getOutputsInfo());
		InferenceEngine::SizeVector outputDims;
		for (auto &item : output_info) {
			auto output_data = item.second;
			outputName = item.first;
			output_data->setPrecision(Precision::FP32);
			output_data->setLayout(Layout::NCHW);
			outputDims = output_data->getDims();
		}
		maxResultCount = outputDims[2];
		if(maxResultCount>20){
			maxResultCount = 20;
		}
		return net;

	}
	int maxResultCount;
	int objectSize;
	string input;
	ExecutableNetwork network;
	InferRequest::Ptr request;
	string inputName;
	string outputName;

	vector<ObjectResult> GetResults(FrameData& data, const float * res) {
		vector<ObjectResult> items;

		for (int x = 0; x < maxResultCount; x++) {
			try {
				ObjectResult item;
				item.confidence = res[x * 7 + 2];
				if (item.confidence > 0.5) {
					item.label = res[x * 7 + 1];
					item.location.x = res[x * 7 + 3] * data.width;;
					item.location.y = res[x * 7 + 4] * data.height;;
					item.location.width = res[x * 7 + 5] * data.width - item.location.x ;
					item.location.height = res[x * 7 + 6] * data.height - item.location.y;
					items.push_back(item);
				}
			} catch (...) {
				continue;
			}
		}

		return items;
	}

	void ProcessFrame(FrameData& data) {
		if (!request) {
			request = network.CreateInferRequestPtr();
		}
		auto copy = Mat(data.Image);
		frameToBlob(copy, request, inputName);

		request->Infer();

		auto buffer = request->GetBlob(outputName)->buffer();

		const float *detections = buffer.as<float *>();
		auto items = GetResults(data, detections);
		for (auto& item : items) {

			try {
				string label;
				auto color = item.label == 15 ? _colorRed : _colorPurple;
				if (item.label > 0 && item.label < 21) {
					label = labels[item.label];
				} else {
					label = "Unknown";
				}
				rectangle(data.Image, item.location, color, 2);
				textRectTop(data.Image, item.location, label, color,
						_colorWhite);

			} catch (...) {
				cout << "error: " << item.location << endl;
			}
		}
	}

private:
	string modelBase = "./models/mobilenet-ssd";
	string modelXml = modelBase + ".xml";
	string modelBin = modelBase + ".bin";

	vector<string> labels = { "background", "aeroplane", "bicycle", "bird",
			"boat", "bottle", "bus", "car", "cat", "chair", "cow",
			"diningtable", "dog", "horse", "motorbike", "person", "pottedplant",
			"sheep", "sofa", "train", "tvmonitor" };
};
