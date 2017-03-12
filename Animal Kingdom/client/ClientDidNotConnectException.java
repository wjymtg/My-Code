package client;

import javax.swing.JOptionPane;

public class ClientDidNotConnectException extends Exception {

	private static final long serialVersionUID = 1L;
	
	public ClientDidNotConnectException(String message) {
		super(message);
        JOptionPane.showMessageDialog(null, message);
	}

}
