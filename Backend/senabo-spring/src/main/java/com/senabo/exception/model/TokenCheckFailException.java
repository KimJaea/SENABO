package com.senabo.exception.model;


import com.senabo.exception.message.ExceptionMessage;
import lombok.Getter;
import org.springframework.http.HttpStatus;
@Getter
public class TokenCheckFailException extends RuntimeException{
    private HttpStatus httpStatus;
    private ExceptionMessage exceptionMessage;
    private String message;
    public TokenCheckFailException(ExceptionMessage exceptionMessage) {
      this.httpStatus = exceptionMessage.getHttpStatus();
      this.exceptionMessage = exceptionMessage;
      this.message = exceptionMessage.getMessage();
    }
}
