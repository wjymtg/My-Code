package packet;

public class UserAuthenticationError {
	
	private String response;
	
	public UserAuthenticationError(String response) {
		this.response = response;
	}
	
	UserAuthenticationError() {}

	public String getResponse() {
		return response;
	}

	public void setResponse(String response) {
		this.response = response;
	}

}
