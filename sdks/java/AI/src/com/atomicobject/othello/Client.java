package com.atomicobject.othello;
import java.io.BufferedReader;
import java.io.Closeable;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.Socket;

import com.google.gson.Gson;

public class Client {

	BufferedReader input;
	OutputStreamWriter out;
	Gson gson = new Gson();
	AI ai;

	public Client(Socket socket, int[][] moves) {
		try {
			ai = new AI(moves);
			input = new BufferedReader(new InputStreamReader(socket.getInputStream()));
			out = new OutputStreamWriter(socket.getOutputStream());
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public void start() {
		System.out.println("Starting client processing ...");
		GameState state;
		try {
			while ((state = readStateFromServer()) != null) {
				int[] move = ai.computeMove(state);
				respondWithMove(move);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		closeStreams();
	}

	private GameState readStateFromServer() throws IOException {
		System.out.println("Reading from server ...");
		String nextLine = input.readLine();
		System.out.println("Read data: " + nextLine);
		if (nextLine == null) return null;
		return gson.fromJson(nextLine.trim(), GameState.class);
	}

	private void respondWithMove(int[] move) throws IOException {
		String encoded = gson.toJson(move);
		System.out.println("Sending response: " + encoded);
		out.write(encoded);
		out.write("\n");
		out.flush();
	}

	private void closeStreams() {
		closeQuietly(input);
		closeQuietly(out);
	}

	private void closeQuietly(Closeable stream) {
		try {
			stream.close();
		} catch (IOException e1) {
			e1.printStackTrace();
		}
	}
}
