#pragma once

#include <gflags/gflags.h>
#include <functional>
#include <iostream>
#include <string>
#include <utility>
#include <algorithm>
#include <vector>
#include <inference_engine.hpp>
#include <ie_iextension.h>
#include <opencv2/opencv.hpp>
#include "common.hpp"

using namespace InferenceEngine;
using namespace cv;
using namespace std;

struct EmotionsResult {
	float age;
	bool isMale;
};

class OpenVinoEmotionsRecognizer {
public:
	OpenVinoEmotionsRecognizer() {
	}

	OpenVinoEmotionsRecognizer(InferencePlugin& plugin) {
		map<string, string> config;
		network = plugin.LoadNetwork(InitNetwork(), config);
	}

	CNNNetwork InitNetwork() {
		CNNNetReader netReader;
		netReader.ReadNetwork(modelXml);
		netReader.getNetwork().setBatchSize(1);
		netReader.ReadWeights(modelBin);
		InputsDataMap inputInfo(netReader.getNetwork().getInputsInfo());
		auto& inputInfoFirst = inputInfo.begin()->second;
		inputInfoFirst->setPrecision(Precision::U8);
		input = inputInfo.begin()->first;
		OutputsDataMap outputInfo(netReader.getNetwork().getOutputsInfo());
		for (auto& output : outputInfo) {
			output.second->setPrecision(Precision::FP32);
		}

		DataPtr emotionsOutput = outputInfo.begin()->second;
		auto emotionsCreatorLayer = emotionsOutput->getCreatorLayer().lock();
		outputEmotions = emotionsOutput->name;
		return netReader.getNetwork();

	}
	string input;
	ExecutableNetwork network;
	InferRequest::Ptr request;
	string inputName;
	string outputEmotions;

	string GetResult(Mat& frame) {
		if (!request) {
			request = network.CreateInferRequestPtr();
		}
		frameToBlob(frame, request, input);
		request->Infer();
		static const vector<string> emotions = { "neutral", "happy", "sad",
				"surprise", "anger" };
		auto emotionsVecSize = emotions.size();

		Blob::Ptr emotionsBlob = request->GetBlob(outputEmotions);

		auto emotionsValues = emotionsBlob->buffer().as<float *>();
		auto outputIdxPos = emotionsValues + 0;

		int maxProbEmotionIx = max_element(outputIdxPos,
				outputIdxPos + emotionsVecSize) - outputIdxPos;
		return emotions[maxProbEmotionIx];
	}

private:
	string modelBase = "./models/emotions-recognition-retail-0003";
	string modelXml = modelBase + ".xml";
	string modelBin = modelBase + ".bin";
};
