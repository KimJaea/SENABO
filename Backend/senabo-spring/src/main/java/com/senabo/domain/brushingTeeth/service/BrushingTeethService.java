package com.senabo.domain.brushingTeeth.service;

import com.senabo.domain.brushingTeeth.dto.response.BrushingTeethResponse;
import com.senabo.domain.brushingTeeth.dto.response.CheckBrushingTeethResponse;
import com.senabo.domain.brushingTeeth.entity.BrushingTeeth;
import com.senabo.domain.brushingTeeth.repository.BrushingTeethRepository;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
import com.senabo.exception.message.ExceptionMessage;
import com.senabo.exception.model.DataException;
import com.senabo.exception.model.UserException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.sql.Time;
import java.sql.Timestamp;
import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@Slf4j
@Service
@Transactional(readOnly = true)
@RequiredArgsConstructor
public class BrushingTeethService {
    private final BrushingTeethRepository brushingTeethRepository;
    private final MemberService memberService;

    @Transactional
    public BrushingTeethResponse createBrushingTeeth(String email) {
        Member member = memberService.findByEmail(email);
        BrushingTeeth brushingTeeth = brushingTeethRepository.save(
                new BrushingTeeth(member));
        try {
            brushingTeethRepository.flush();
        } catch (DataIntegrityViolationException e) {
            throw new UserException(String.valueOf(ExceptionMessage.FAIL_SAVE_DATA));
        }
        return BrushingTeethResponse.from(brushingTeeth);
    }

    @Transactional
    public List<BrushingTeeth> getBrushingTeeth(String email) {
        Member member = memberService.findByEmail(email);
        List<BrushingTeeth> brushingTeethList = brushingTeethRepository.findByMemberId(member);
        return brushingTeethList;
    }
    @Transactional
    public List<BrushingTeeth> getBrushingTeethWeek(Report report, Member member) {
        LocalDateTime startTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS);
        LocalDateTime endTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS).plusDays(7);
        List<BrushingTeeth> brushingTeethList = brushingTeethRepository.findBrushingTeethWeek(member, endTime, startTime);
        return brushingTeethList;
    }

    @Transactional
    public void removeBrushingTeeth(String email) {
        try {
            Member member = memberService.findByEmail(email);
            List<BrushingTeeth> list = brushingTeethRepository.deleteByMemberId(member);
        } catch (DataIntegrityViolationException e) {
            throw new UserException(ExceptionMessage.FAIL_DELETE_DATA);
        }
    }

    public CheckBrushingTeethResponse checkBrushingTeeth(Report report, Member member) {
        try {
            log.info("=====================================");
            // 최신 주간 리포트 확인 후 start Date 가져오기



            LocalDateTime startTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS);

            Long countWeek = brushingTeethRepository.countBrushingTeethWeek(member, startTime);
            log.info("countWeek: {}", countWeek);

            Long countToday = brushingTeethRepository.countBrushingTeethToday(member);
            log.info("countToday: {}", countToday);
            log.info("=====================================");

            boolean possibleYn = false;
            if(countWeek < 3 && countToday == 0){
                possibleYn = true;
            }
            return CheckBrushingTeethResponse.from(possibleYn);
        } catch (DataIntegrityViolationException e) {
            throw new DataException(ExceptionMessage.DATA_NOT_FOUND);
        }
    }
}
