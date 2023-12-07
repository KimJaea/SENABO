package com.senabo.domain.bath.service;

import com.senabo.domain.bath.dto.response.BathResponse;
import com.senabo.domain.bath.entity.Bath;
import com.senabo.domain.bath.repository.BathRepository;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.exception.message.ExceptionMessage;
import com.senabo.exception.model.UserException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Slf4j
@Service
@Transactional(readOnly = true)
@RequiredArgsConstructor
public class BathService {
    private final BathRepository bathRepository;
    private final MemberService memberService;

    @Transactional
    public BathResponse createBath(String email) {
        Member member = memberService.findByEmail(email);
        Bath bath = bathRepository.save(
                new Bath(member));
        try {
            bathRepository.flush();
        } catch (DataIntegrityViolationException e) {
            throw new UserException(String.valueOf(ExceptionMessage.FAIL_SAVE_DATA));
        }
        return BathResponse.from(bath);
    }

    @Transactional
    public List<Bath> getBath(String email) {
        Member member = memberService.findByEmail(email);
        List<Bath> bathList = bathRepository.findByMemberId(member);
        return bathList;
    }

    @Transactional
    public void removeBath(String email) {
        try {
            Member member = memberService.findByEmail(email);
            List<Bath> list = bathRepository.deleteByMemberId(member);
        } catch (DataIntegrityViolationException e) {
            throw new UserException(ExceptionMessage.FAIL_DELETE_DATA);
        }
    }

}