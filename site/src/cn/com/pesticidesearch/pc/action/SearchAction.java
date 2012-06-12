package cn.com.pesticidesearch.pc.action;

import com.opensymphony.xwork2.ActionSupport;

 
public class SearchAction extends ActionSupport {

	/**
	 * 
	 */
	private static final long serialVersionUID = -3378530817495711951L;

	private String searchtext;
	
	 public String getSearchtext() {
		return searchtext;
	}

	public void setSearchtext(String searchtext) {
		this.searchtext = searchtext;
	}

	public String execute() throws Exception {
 
	        return SUCCESS;
	    }

 

 

 

}
