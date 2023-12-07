package com.senabo.common.message;

import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

@Slf4j
@Service
public class ParsingMessageService {

    public String parseLastCharacter(String input) {
        // 문자열이 비어있는 경우 예외처리
        if (input.isEmpty()) {
            log.info("입력된 문자열이 비어있습니다.");
            return "";
        }
        String dogName = input;
        // 문자열의 마지막 문자 추출
        char lastChar = input.charAt(input.length() - 1);

        if (lastChar < 0xAC00 || lastChar > 0xD7A3)
            return input;

        String select = ((lastChar - 0xAC00) % 28 > 0) ? "이" : "";
        return input + select;
    }
}